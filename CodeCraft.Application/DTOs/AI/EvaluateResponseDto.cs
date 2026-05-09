using System.Text.Json.Serialization;

namespace CodeCraft.Application.DTOs.AI;

public class EvaluateResponseDto
{
    [JsonPropertyName("score")]
    public double Score { get; set; }

    [JsonPropertyName("level")]
    public string? Level { get; set; }

    [JsonPropertyName("result")]
    public List<EvaluationResultItemDto> Result { get; set; } = new();
}

public class EvaluationResultItemDto
{
    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("similarity")]
    public double Similarity { get; set; }

    [JsonPropertyName("points")]
    public double Points { get; set; }
}