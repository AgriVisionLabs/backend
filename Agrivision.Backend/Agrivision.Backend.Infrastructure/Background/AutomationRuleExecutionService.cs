using Agrivision.Backend.Application.Services.IoT;
using Agrivision.Backend.Domain.Enums.Core;
using Agrivision.Backend.Infrastructure.Cache;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Infrastructure.Background;

public class AutomationRuleExecutionService(IServiceScopeFactory scopeFactory, ILogger<AutomationRuleExecutionService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("🟢 AutomationRuleExecutionService started at {Time}", DateTime.UtcNow);

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("🔄 Starting automation rules check loop at {Time}", DateTime.UtcNow);

            try
            {
                await CheckAndTriggerThresholdRulesAsync();

                await CheckAndTriggerScheduledRulesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ Unhandled error while executing automation rule logic at {Time}", DateTime.UtcNow);
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }

        logger.LogWarning("🛑 AutomationRuleExecutionService was stopped at {Time}", DateTime.UtcNow);
    }
    
    private async Task CheckAndTriggerThresholdRulesAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
        var communicator = scope.ServiceProvider.GetRequiredService<IWebSocketDeviceCommunicator>();

        var rules = await db.AutomationRules
            .Include(r => r.SensorUnit)
            .Include(r => r.IrrigationUnit)
            .Where(r =>
                !r.IsDeleted &&
                r.IsEnabled &&
                r.Type == AutomationRuleType.Threshold &&
                r.TargetSensorType != null)
            .ToListAsync();

        foreach (var rule in rules)
        {
            var sensor = rule.SensorUnit;
            var unit = rule.IrrigationUnit;

            float? value = LiveSensorCache.TryGetLatest(
                sensor.DeviceId,
                rule.TargetSensorType!.Value,
                TimeSpan.FromSeconds(30)
            );

            if (value is null)
            {
                value = await db.SensorReadings
                    .Where(r =>
                        r.SensorConfiguration.DeviceId == sensor.DeviceId &&
                        r.Type == rule.TargetSensorType)
                    .OrderByDescending(r => r.TimeStamp)
                    .Select(r => (float?)r.Value)
                    .FirstOrDefaultAsync();
            }

            if (value is null)
            {
                logger.LogWarning("No recent reading for sensor {SensorId}, rule '{RuleName}'", sensor.Id, rule.Name);
                continue;
            }

            bool shouldToggle = (rule.MinimumThresholdValue.HasValue && value < rule.MinimumThresholdValue && !unit.IsOn) ||
                                (rule.MaximumThresholdValue.HasValue && value > rule.MaximumThresholdValue && unit.IsOn);

            if (!shouldToggle)
            {
                logger.LogInformation("Rule '{RuleName}' skipped — value {Value} within thresholds.", rule.Name, value);
                continue;
            }

            if (!unit.IsOnline || !communicator.IsDeviceConnected(unit.DeviceId))
            {
                logger.LogWarning("Irrigation unit {UnitId} offline/disconnected for rule '{RuleName}'", unit.Id, rule.Name);
                continue;
            }

            var commandSent = await communicator.SendCommandAsync(unit.DeviceId, "toggle_pump");

            if (!commandSent)
            {
                logger.LogError("Failed to send toggle to unit {UnitId} for rule '{RuleName}'", unit.Id, rule.Name);
                continue;
            }

            // apply same state update logic
            var now = DateTime.UtcNow;
            unit.IsOn = !unit.IsOn;
            unit.ToggledById = Guid.Empty.ToString(); // "00000000-0000-0000-0000-000000000000"
            unit.UpdatedById = Guid.Empty.ToString();
            unit.UpdatedOn = now;

            if (unit.IsOn)
                unit.LastActivation = now;
            else
                unit.LastDeactivation = now;

            db.IrrigationUnits.Update(unit);
            await db.SaveChangesAsync();

            await communicator.SendConfirmationAsync(unit.DeviceId, "toggle_pump");

            logger.LogInformation("✅ Rule '{RuleName}' toggled unit {UnitId}", rule.Name, unit.Id);
        }
    }
    
    private async Task CheckAndTriggerScheduledRulesAsync()
    {
        using var scope = scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
        var communicator = scope.ServiceProvider.GetRequiredService<IWebSocketDeviceCommunicator>();

        var now = DateTime.UtcNow.AddHours(3);
        var nowTime = TimeOnly.FromDateTime(now);
        var todayBit = (DaysOfWeek)(1 << (int)now.DayOfWeek);

        var tolerance = TimeSpan.FromSeconds(20);

        var rules = await db.AutomationRules
            .Include(r => r.IrrigationUnit)
            .Where(r =>
                !r.IsDeleted &&
                r.IsEnabled &&
                r.Type == AutomationRuleType.Scheduled &&
                r.StartTime != null &&
                r.EndTime != null &&
                ((DaysOfWeek)r.ActiveDays!).HasFlag(todayBit))
            .ToListAsync();

        foreach (var rule in rules)
        {
            var unit = rule.IrrigationUnit;

            if (!unit.IsOnline || !communicator.IsDeviceConnected(unit.DeviceId))
            {
                logger.LogWarning("Unit {UnitId} is offline or disconnected for rule '{RuleName}'", unit.Id, rule.Name);
                continue;
            }

            var shouldStart = IsTimeNear(rule.StartTime!.Value, nowTime, tolerance);
            var shouldStop = IsTimeNear(rule.EndTime!.Value, nowTime, tolerance);
            var nowUtc = DateTime.UtcNow;

            if (shouldStart && !unit.IsOn)
            {
                var success = await communicator.SendCommandAsync(unit.DeviceId, "toggle_pump");
                if (success)
                {
                    unit.IsOn = true;
                    unit.ToggledById = Guid.Empty.ToString();
                    unit.UpdatedById = Guid.Empty.ToString();
                    unit.UpdatedOn = nowUtc;
                    unit.LastActivation = nowUtc;

                    await communicator.SendConfirmationAsync(unit.DeviceId, "toggle_pump");

                    db.IrrigationUnits.Update(unit);
                    logger.LogCritical("🟢 Scheduled rule '{RuleName}' started unit {UnitId}", rule.Name, unit.Id);
                }
                else
                {
                    logger.LogError("❌ Failed to start unit {UnitId} for rule '{RuleName}'", unit.Id, rule.Name);
                }
            }
            else if (shouldStop && unit.IsOn)
            {
                var success = await communicator.SendCommandAsync(unit.DeviceId, "toggle_pump");
                if (success)
                {
                    unit.IsOn = false;
                    unit.ToggledById = Guid.Empty.ToString();
                    unit.UpdatedById = Guid.Empty.ToString();
                    unit.UpdatedOn = nowUtc;
                    unit.LastDeactivation = nowUtc;

                    await communicator.SendConfirmationAsync(unit.DeviceId, "toggle_pump");

                    db.IrrigationUnits.Update(unit);
                    logger.LogInformation("🔴 Scheduled rule '{RuleName}' stopped unit {UnitId}", rule.Name, unit.Id);
                }
                else
                {
                    logger.LogError("❌ Failed to stop unit {UnitId} for rule '{RuleName}'", unit.Id, rule.Name);
                }
            }
        }

        await db.SaveChangesAsync();
    }

    private bool IsTimeNear(TimeOnly target, TimeOnly now, TimeSpan window)
    {
        var diff = Math.Abs((target.ToTimeSpan() - now.ToTimeSpan()).TotalSeconds);
        return diff <= window.TotalSeconds;
    }
}