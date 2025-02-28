using System.IO.Compression;
using System.Text;
using Agrivision.Backend.Application.Services.Utility;
using Microsoft.AspNetCore.WebUtilities;

namespace Agrivision.Backend.Infrastructure.Services.Utility;

public class UtilityService : IUtilityService
{
    public string Encode(string input)
    {
        return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(input));
    }

    public bool TryDecode(string encodedInput, out string? decodedOutput)
    {
        try
        {
            decodedOutput = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedInput));
            return true;
        }
        catch(FormatException)
        {
            decodedOutput = null;
            return false;
        }
        
    }
}