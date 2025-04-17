
using Agrivision.Backend.Application.Auth;
using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Auth.Commands;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;

namespace Agrivision.Backend.Application.Features.Auth.Handlers;
public class VerifyOtpCommandHandler(IOtpProvider otpProvider) : IRequestHandler<VerifyOtpCommand, Result>
{
    public async Task<Result> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        // Verify the OTP using the email and OTP provided
        var isValid = await otpProvider.VerifyOtpAsync(request.Email, request.Otp, cancellationToken);
        if (!isValid)
            return Result.Failure(OtpErrors.InvalidOtp);
     

        return Result.Success();
    }

}
