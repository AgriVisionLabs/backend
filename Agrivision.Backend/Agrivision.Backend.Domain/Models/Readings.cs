using System.Text.Json.Serialization;

namespace Agrivision.Backend.Domain.Models;

public class Readings
{
    [JsonPropertyName("moisture")]
    public string Moisture { get; set; }

    [JsonPropertyName("temperature")]
    public string Temperature { get; set; }

    [JsonPropertyName("humidity")]
    public string Humidity { get; set; }
}