using Agrivision.Backend.Application.Errors;
using Agrivision.Backend.Application.Features.Account.Commands;
using Agrivision.Backend.Application.Repositories.Identity;
using Agrivision.Backend.Application.Services.Files;
using Agrivision.Backend.Application.Settings;
using Agrivision.Backend.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Options;

namespace Agrivision.Backend.Application.Features.Account.Handlers;

public class UpdatePfpCommandHandler(IUserRepository userRepository, IFileUploadService fileUploadService, IOptions<ServerSettings> serverSettings) : IRequestHandler<UpdatePfpCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdatePfpCommand request, CancellationToken cancellationToken)
    {   
        var user = await userRepository.FindByIdAsync(request.UserId);
        if (user is null)
            return Result.Failure<string>(UserErrors.UserNotFound);
        
        var filename = await fileUploadService.UploadImageAsync(request.Image);
        user.PfpImageUrl = $"{serverSettings.Value.BaseUrl}/uploads/{filename}";
        
        user.UpdatedAt = DateTime.UtcNow;
        await userRepository.UpdateAsync(user);
        
        return Result.Success(user.PfpImageUrl);
    }
}