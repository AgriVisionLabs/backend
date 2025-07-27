using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Agrivision.Backend.Application.Services.Hubs;
using Agrivision.Backend.Domain.Entities.Core;
using Agrivision.Backend.Domain.Enums.Core;
using Agrivision.Backend.Application.Repositories.Core;

namespace Agrivision.Backend.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController(
    INotificationBroadcaster notificationBroadcaster, 
    INotificationRepository notificationRepository) : ControllerBase
{
    [HttpGet("test/{userId}")]
    public async Task<IActionResult> SendTestNotification(string userId, [FromServices] IFarmUserRoleRepository farmUserRoleRepository)
    {
        // Get user's first farm
        var userFarms = await farmUserRoleRepository.GetAllAccessible(userId);
        if (!userFarms.Any())
        {
            return BadRequest("User has no farms to send notification to");
        }

        var firstFarm = userFarms.First();
        
        var testNotification = new Notification
        {
            Id = Guid.NewGuid(),
            Type = NotificationType.Alert,
            Message = "🧪 This is a test notification sent manually!",
            FarmId = firstFarm.FarmId, // Use actual farm ID
            FieldId = null,
            CreatedById = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system",
            CreatedOn = DateTime.UtcNow,
            UserIds = new List<string> { userId }
        };

        // Save to database
        await notificationRepository.AddAsync(testNotification);

        // Broadcast to SignalR
        await notificationBroadcaster.BroadcastNotificationAsync(userId, testNotification);

        return Ok(new { 
            message = "Test notification sent!", 
            notificationId = testNotification.Id,
            targetUserId = userId,
            farmId = firstFarm.FarmId,
            farmName = firstFarm.Farm?.Name
        });
    }

    [HttpGet("user-farms")]
    public async Task<IActionResult> GetUserFarms([FromServices] IFarmUserRoleRepository farmUserRoleRepository)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var userRoles = await farmUserRoleRepository.GetAllAccessible(userId);
        
        return Ok(new {
            userId,
            farmCount = userRoles.Count,
            farms = userRoles.Select(ur => new {
                farmId = ur.FarmId,
                farmName = ur.Farm?.Name ?? "N/A",
                role = ur.FarmRole?.Name ?? "N/A",
                canReceiveNotifications = ur.FarmRole?.Name is "Manager" or "Owner"
            })
        });
    }
} 