using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class FarmInvitationErrors
{
    public static readonly Error InvitationAlreadyExists = new(
        "Invitation.AlreadyExists",
        "An active invitation has already been sent to this user for this farm.");

    public static readonly Error SelfInvitation = new("Invitation.SelfInvitation",
        "You cannot invite a user who already has access to the farm.");
}