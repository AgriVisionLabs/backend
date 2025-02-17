namespace Agrivision.Backend.Application.Abstractions;

public record Error(string Title, string Detail)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}