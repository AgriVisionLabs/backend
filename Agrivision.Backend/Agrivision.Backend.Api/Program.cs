using Agrivision.Backend.Api;
using Agrivision.Backend.Infrastructure;
using Agrivision.Backend.Infrastructure.Persistence.Core;
using Agrivision.Backend.Infrastructure.Persistence.Identity;
using Agrivision.Backend.Infrastructure.Seeding;
using Agrivision.Backend.Infrastructure.WebSockets;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDependencies(builder.Configuration);

builder.Host.AddSerilog();

var app = builder.Build();

// seed the database(s)
// using (var scope = app.Services.CreateScope())
// {
//     // seed identity
//     var identityDbContext = scope.ServiceProvider.GetRequiredService<ApplicationUserDbContext>();
//     identityDbContext.Database.Migrate();
//     await IdentitySeeder.SeedGlobalRolesAsync(scope.ServiceProvider);
//     await IdentitySeeder.SeedAdminUserAsync(scope.ServiceProvider);
//     await IdentitySeeder.SeedGlobalRolePermissionAsync(scope.ServiceProvider);
//     
//     // seed core
//     var coreDbContext = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
//     coreDbContext.Database.Migrate();
//     await CoreSeeder.SeedRolesAsync(scope.ServiceProvider);
//     await CoreSeeder.SeedDemoFarmAsync(scope.ServiceProvider);
//     await CoreSeeder.SeedCoreRolePermissionAsync(scope.ServiceProvider);
//     await CoreSeeder.SeedDevicesAsync(scope.ServiceProvider);
// }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || builder.Configuration.GetValue<bool>("ApiDocumentation:Enabled")) 
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Agrivision API");
        options.RoutePrefix = "swagger";
    });
}

app.UseWebSockets();

app.Map("/IrrigationDeviceWS", async context =>
{
    var handler = context.RequestServices.GetRequiredService<IrrigationUnitDeviceWebSocketHandler>();
    await handler.HandleAsync(context);
});

app.Map("/SensorDeviceWS", async context =>
{
    var handler = context.RequestServices.GetRequiredService<SensorUnitDeviceWebSocketHandler>();
    await handler.HandleAsync(context);
});

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAny");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseExceptionHandler();

app.Run();