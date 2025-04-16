using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class FarmInvitationErrors
{
    public static readonly Error InvitationAlreadyExists = new(
        "Invitation.AlreadyExists",
        "An active invitation has already been sent to this user for this farm.");

    public static readonly Error SelfInvitation = new("Invitation.SelfInvitation",
        "You cannot invite a user who already has access to the farm.");
    
    public static readonly Error CannotInviteAsOwner = new(
        "Invitation.CannotInviteAsOwner",
        "Users cannot be invited with the Owner role.");
    
    public static readonly Error InvalidToken = new(
        "FarmInvitation.InvalidToken",
        "The invitation token is invalid or has expired.");
}