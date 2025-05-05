using Agrivision.Backend.Application.Features.Hubs.Commands;
using Agrivision.Backend.Application.Repositories.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Agrivision.Backend.Api.Hubs;
 
 [Authorize]
 public class SensorHub(IMediator mediator, ISensorUnitRepository sensorUnitRepository, ILogger<SensorHub> logger) : Hub
 {
     public override async Task OnDisconnectedAsync(Exception? exception)
     {
         await mediator.Send(new UnsubscribeFromFarmCommand(Context.ConnectionId));
         await base.OnDisconnectedAsync(exception);
     }
     
     public async Task SubscribeToFarm(Guid farmId)
     {
         var userId = Context.UserIdentifier;

         if (string.IsNullOrWhiteSpace(userId))
         {
             Context.Abort();
             return;
         }

         var result = await mediator.Send(new SubscribeToFarmCommand(
             farmId,
             Context.ConnectionId,
             userId
         ));

         if (!result.Succeeded)
         {
             throw new HubException(result.Error.ToString());
         }

         logger.LogInformation("👤 {UserId} subscribed to farm {FarmId}", userId, farmId);
     }
 }