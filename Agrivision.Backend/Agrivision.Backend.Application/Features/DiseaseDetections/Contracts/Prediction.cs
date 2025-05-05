
using System.Text.Json.Serialization;

namespace Agrivision.Backend.Application.Features.DiseaseDetections.Contracts;
public class Prediction
{
    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }

    [JsonPropertyName("width")]
    public float Width { get; set; }

    [JsonPropertyName("height")]
    public float Height { get; set; }

    [JsonPropertyName("confidence")]
    public float Confidence { get; set; }

    [JsonPropertyName("class")]
    public string Class { get; set; }
    [JsonPropertyName("class_id")]
    public int ClassId { get; set; }

    [JsonPropertyName("detection_id")]
    public string DetectionId { get; set; }
}

