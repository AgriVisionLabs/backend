namespace Agrivision.Backend.Application.Services.Otp;

public interface IOtpGenerator
{
    string GenerateCode(int length = 6);
}