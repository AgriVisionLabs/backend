using Agrivision.Backend.Application.Services.Files;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Agrivision.Backend.Infrastructure.Services.Files;

public class FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger) : IFileUploadService
{
    public async Task<string> UploadImageAsync(IFormFile file)
    {
        var allowedExtensions = new List<string>{
            ".jpg", ".jpeg", ".png"
        };

        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or null");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
            throw new InvalidOperationException("Unsupported file extension");

        if (!await IsValidImageSignatureAsync(file))
            throw new InvalidOperationException("File signature does not match known image formats");

        var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueName = Guid.NewGuid() + ext;
        var filePath = Path.Combine(uploadsFolder, uniqueName);
        
        logger.LogCritical(filePath);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return uniqueName;
    }

    private async Task<bool> IsValidImageSignatureAsync(IFormFile file)
    {
        byte[] buffer = new byte[8];
        await using (var stream = file.OpenReadStream())
        {
            await stream.ReadExactlyAsync(buffer, 0, buffer.Length);
        }

        // JPEG
        if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
            return true;

        // PNG
        if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E &&
            buffer[3] == 0x47 && buffer[4] == 0x0D && buffer[5] == 0x0A &&
            buffer[6] == 0x1A && buffer[7] == 0x0A)
            return true;

        return false;
    }
}