using Agrivision.Backend.Domain.Enums.Core;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Subscription.Commands;

public record UpdateSubscriptionStatusCommand(
    string StripeSubscriptionId,
    UserSubscriptionStatus Status
) : IRequest<Result>;