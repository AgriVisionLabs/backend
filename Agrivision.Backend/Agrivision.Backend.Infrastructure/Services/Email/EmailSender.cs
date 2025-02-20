using System.Collections.Immutable;
using Agrivision.Backend.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Agrivision.Backend.Infrastructure.Services.Email;

public class EmailSender(IOptions<MailSettings> mailSettings, ILogger<EmailSender> logger) : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(mailSettings.Value.SenderName, mailSettings.Value.SenderEmail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };

            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(mailSettings.Value.SmtpServer, mailSettings.Value.Port,
                SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(mailSettings.Value.UserName, mailSettings.Value.Password);
            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true);
            
            logger.LogInformation("Sending an email to {email}", email);
        
            logger.LogInformation("📩 Email sent successfully to {email}", email);
        }
        catch (Exception e)
        {
            logger.LogError("❌ Failed to send email: {message}", e.Message);
        }
    }
}