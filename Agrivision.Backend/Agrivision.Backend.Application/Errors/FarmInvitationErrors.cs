using Agrivision.Backend.Domain.Abstractions;

namespace Agrivision.Backend.Application.Errors;

public static class FarmInvitationErrors
{
    public static readonly Error InvitationAlreadyExists = new(
        "FarmInvitation.AlreadyExists",
        "An active invitation has already been sent to this user for this farm.");

    public static readonly Error SelfInvitation = new("FarmInvitation.SelfInvitation",
        "You cannot invite a user who already has access to the farm.");
    
    public static readonly Error CannotInviteAsOwner = new(
        "FarmInvitation.CannotInviteAsOwner",
        "Users cannot be invited with the Owner role.");
    
    public static readonly Error InvalidToken = new(
        "FarmInvitation.InvalidToken",
        "The invitation token is invalid or has expired.");

    public static readonly Error InvitationNotFound =
        new("FarmInvitation.InvitationNotFound", "The requested invitation is not found.");
    
    public static readonly Error UnauthorizedAction = new("Invitation.UnauthorizedAction", "User is not authorized to perform this action.");
    
    public static readonly Error AlreadyAccepted = new(
        "FarmInvitation.AlreadyAccepted",
        "The invitation has already been accepted and cannot be modified.");
}