
using Agrivision.Backend.Application.Services.FileManagement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Agrivision.Backend.Infrastructure.Services.FileManagement;
public class FileService(IWebHostEnvironment webHostEnvironment):IFileService
{
    private readonly string imagesPath = $"{webHostEnvironment.WebRootPath}/Images";


    public async Task<string> UploadImageAsync(IFormFile image,CancellationToken cancellationToken=default)
    {
        var path=Path.Combine(imagesPath,image.FileName);
     
        using var stream=File.Create(path);
        await image.CopyToAsync(stream,cancellationToken);

        return path;
    }


}
