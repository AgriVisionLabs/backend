namespace Agrivision.Backend.Domain.Abstractions.Consts;

public static class RegexPatterns
{
    public const string Password = "^(?=.*[0-9])(?=.*[\\W])(?=.*[a-z])(?=.*[A-Z]).{8,}$";
    public const string EgyptianPhoneNumber = "^(\\+201|01|00201)[0-2,5]{1}[0-9]{8}";
}