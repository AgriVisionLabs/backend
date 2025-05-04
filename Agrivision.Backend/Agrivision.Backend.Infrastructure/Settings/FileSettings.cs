
namespace Agrivision.Backend.Infrastructure.Settings;
public class FileSettings
{
    public const int MaxFileSizeInMB = 1;

    public const int MaxFileSizeInBytes = MaxFileSizeInMB*1024*1024;

    public static readonly string [] AllowedImagesExtensions=[".png",".jpg",".jpeg",".jfif"];
    
}
