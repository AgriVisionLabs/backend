using Microsoft.AspNetCore.Http;

namespace Agrivision.Backend.Application.Services.Files;

public interface IFileUploadService
{
    Task<string> UploadImageAsync(IFormFile file);
    Task<string> UploadVideoAsync(IFormFile file);
}