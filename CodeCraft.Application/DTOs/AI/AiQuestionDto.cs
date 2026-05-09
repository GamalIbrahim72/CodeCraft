using System.Text.Json.Serialization;

namespace CodeCraft.Application.DTOs.AI;

public class AiQuestionDto
{
    [JsonPropertyName("question_id")]
    public int Question_Id { get; set; }

    [JsonPropertyName("question")]
    public string? Question_Text { get; set; }

    [JsonPropertyName("difficulty")]
    public string? Difficulty_Level { get; set; }

    [JsonPropertyName("topic")]
    public string? Topic_Area { get; set; }

    [JsonPropertyName("track")]
    public string? Track { get; set; }
}