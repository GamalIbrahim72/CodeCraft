using System.Text.Json.Serialization;

namespace CodeCraft.Application.DTOs.AI;

public class EvaluateResponseDto
{
    [JsonPropertyName("track")]
    public string? Track { get; set; }

    [JsonPropertyName("overall")]
    public EvaluationOverallDto? Overall { get; set; }

    [JsonPropertyName("question_breakdown")]
    public List<QuestionBreakdownDto> QuestionBreakdown { get; set; } = new();

    [JsonIgnore]
    public string? Level => Overall?.Level;

    [JsonIgnore]
    public double? Score => Overall?.Score;
}

public class EvaluationOverallDto
{
    [JsonPropertyName("level")]
    public string? Level { get; set; }

    [JsonPropertyName("score")]
    public double? Score { get; set; }
}

public class QuestionBreakdownDto
{
    [JsonPropertyName("question_number")]
    public int QuestionNumber { get; set; }

    [JsonPropertyName("topic")]
    public string? Topic { get; set; }

    [JsonPropertyName("difficulty")]
    public string? Difficulty { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("score")]
    public double? Score { get; set; }
}