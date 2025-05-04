using System.Text.Json.Serialization;

namespace Agrivision.Backend.Domain.Models;

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
    
    [JsonPropertyName("command")]
    public string Command { get; set; }
    
    [JsonPropertyName("cid")]
    public string Cid { get; set; }
    
    [JsonPropertyName("readings")]
    public Readings Readings { get; set; }
}