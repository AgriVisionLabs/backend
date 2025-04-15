using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Core;
using MediatR;

namespace Agrivision.Backend.Application.Features.Farm.Commands;

public record InviteMemberCommand
(
    string SenderId,
    string SenderName,
    Guid FarmId,
    string Recipient, // username or email
    int RoleId
) : IRequest<Result>;