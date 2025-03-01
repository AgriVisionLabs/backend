namespace Agrivision.Backend.Application.Services.Utility;

public interface IUtilityService
{
    string Encode(string input);
    bool TryDecode(string encodedInput, out string? decodedOutput);
}