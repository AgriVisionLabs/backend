namespace Agrivision.Backend.Application.Services.Email;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string htmlMessage);
    Task SendConfirmationEmail(string email, string token);
    Task SendPasswordResetEmailAsync(string email, string otp);
    Task SendMfaEmailAsync(string email, string otp);
    Task SendInvitationEmail(string farmName, string senderName, string recipientEmail, string token);
}