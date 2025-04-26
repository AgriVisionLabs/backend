using MediatR;
using Agrivision.Backend.Domain.Abstractions;
namespace Agrivision.Backend.Application.Features.Subscription.Commands;
public record CreatePaymentIntentCommand
(
    string UserId,
    Guid PlanId
) : IRequest<Result<string>>; // Returns client_secret
