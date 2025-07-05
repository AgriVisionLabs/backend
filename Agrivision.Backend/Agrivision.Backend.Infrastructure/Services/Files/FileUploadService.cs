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

        if (file.Length == 0)
        {
            logger.LogWarning("Image file is empty or null");
            return string.Empty;
        }

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
        {
            logger.LogWarning("Unsupported image file extension: {Extension}", ext);
            return string.Empty;
        }

        if (!await IsValidImageSignatureAsync(file))
        {
            logger.LogWarning("Image file signature does not match known formats");
            return string.Empty;
        }

        var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueName = Guid.NewGuid() + ext;
        var filePath = Path.Combine(uploadsFolder, uniqueName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return uniqueName;
    }

    public async Task<string> UploadVideoAsync(IFormFile file)
    {
        var allowedExtensions = new List<string>{
            ".mp4"
        };

        if (file.Length == 0)
        {
            logger.LogWarning("Video file is empty or null");
            return string.Empty;
        }

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
        {
            logger.LogWarning("Unsupported video file extension: {Extension}", ext);
            return string.Empty;
        }

        if (!await IsValidVideoSignatureAsync(file))
        {
            logger.LogWarning("Video file signature does not match known formats");
            return string.Empty;
        }

        var uploadsFolder = Path.Combine(environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadsFolder);

        var uniqueName = Guid.NewGuid() + ext;
        var filePath = Path.Combine(uploadsFolder, uniqueName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

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

    private async Task<bool> IsValidVideoSignatureAsync(IFormFile file)
    {
        byte[] buffer = new byte[12];
        await using (var stream = file.OpenReadStream())
        {
            await stream.ReadExactlyAsync(buffer, 0, buffer.Length);
        }

        // MP4
        if (buffer[4] == 0x66 && buffer[5] == 0x74 && buffer[6] == 0x79 && buffer[7] == 0x70)
        {
            return true;
        }

        return false;
    }
}