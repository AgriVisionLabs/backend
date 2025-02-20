namespace Agrivision.Backend.Application.Services.Email;

public interface IEmailBodyBuilder
{
    string GenerateEmailBody(string template, Dictionary<string, string> templateModel);
}