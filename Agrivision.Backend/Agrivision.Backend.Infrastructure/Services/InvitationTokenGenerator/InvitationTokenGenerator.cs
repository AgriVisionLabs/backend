using System.Security.Cryptography;
using Agrivision.Backend.Application.Services.InvitationTokenGenerator;
using Microsoft.AspNetCore.WebUtilities;

namespace Agrivision.Backend.Infrastructure.Services.InvitationTokenGenerator;

public class InvitationTokenGenerator : IInvitationTokenGenerator
{
    public string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return WebEncoders.Base64UrlEncode(bytes);
    }
}