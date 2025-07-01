namespace Agrivision.Backend.Domain.Models;

public class DiseasePredictionResponse
{
    public string Label { get; set; }
    public List<LabelConfidence> Confidences { get; set; }
}