
using Microsoft.AspNetCore.Http;

namespace Agrivision.Backend.Application.Services.FileManagement;
public interface IFileService
{
    Task<string> UploadImageAsync(IFormFile image, CancellationToken cancellationToken = default);
}
