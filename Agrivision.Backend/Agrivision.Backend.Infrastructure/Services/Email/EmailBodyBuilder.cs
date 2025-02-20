using System.Reflection;
using Agrivision.Backend.Application.Services.Email;

namespace Agrivision.Backend.Infrastructure.Services.Email;

public class EmailBodyBuilder : IEmailBodyBuilder
{
    public string GenerateEmailBody(string template, Dictionary<string, string> templateModel)
    {
        var templatePath = $"/Users/youssef/Developer/Projects/Agrivision/backend/Agrivision.Backend/Agrivision.Backend.Infrastructure/Templates/{template}.html";
        var streamReader = new StreamReader(templatePath);
        var body = streamReader.ReadToEnd();
        streamReader.Close();

        foreach (var item in templateModel)
            body = body.Replace(item.Key, item.Value);

        return body;
    }
}