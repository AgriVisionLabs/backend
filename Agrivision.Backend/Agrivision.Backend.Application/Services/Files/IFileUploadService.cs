using Microsoft.AspNetCore.Http;

namespace Agrivision.Backend.Application.Services.Files;

public interface IFileUploadService
{
    Task<string> UploadImageAsync(IFormFile file);
}