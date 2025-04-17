using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Domain.Abstractions;
using Agrivision.Backend.Domain.Entities.Identity;

namespace Agrivision.Backend.Infrastructure.Auth;
public class OtpProvider(IOtpVerificationRepository otpRepository) : IOtpProvider
{
    public string GenerateOtp()
    {
        // Generate a 6-digit OTP
        var random = new Random();
        return random.Next(100000, 999999).ToString();
    }

    public async Task StoreOtpAsync(string email, string otp, CancellationToken cancellationToken)
    {
        var otpVerification = new OtpVerification
        {
            Id = Guid.NewGuid(),
            Email = email,
            OtpCode = otp,
            CreatedOn = DateTime.UtcNow,
            ExpiresOn = DateTime.UtcNow.AddMinutes(10), // OTP valid for 10 minutes
            IsUsed = false
        };

        await otpRepository.AddAsync(otpVerification, cancellationToken);
    }

    public async Task<bool> VerifyOtpAsync(string email, string otp, CancellationToken cancellationToken)
    {
        var otpVerification = await otpRepository.FindByEmailAndOtpAsync(email, otp, cancellationToken);
        if (otpVerification == null || otpVerification.IsUsed || otpVerification.ExpiresOn < DateTime.UtcNow)
            return false;

        
        return true;
    }
    public async Task EndVarification(string email, string otp, CancellationToken cancellationToken)
    {
        var otpVerification = await otpRepository.FindByEmailAndOtpAsync(email, otp, cancellationToken);
       
        otpVerification!.IsUsed = true;
        await otpRepository.UpdateAsync(otpVerification, cancellationToken);
        
    }
}
