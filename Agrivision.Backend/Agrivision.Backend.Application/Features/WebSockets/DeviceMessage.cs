using System.Text.Json.Serialization;

namespace Agrivision.Backend.Application.Features.WebSockets;

public class DeviceMessage
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("macAddress")]
    public string MacAddress { get; set; }

    [JsonPropertyName("ip")]
    public string Ip { get; set; }

    [JsonPropertyName("provisioningKey")]
    public string ProvisioningKey { get; set; }
}