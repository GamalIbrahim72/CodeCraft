using System.Text.Json.Serialization;

namespace CodeCraft.Application.DTOs.AI;

public class EvaluateRequestDto
{
    [JsonPropertyName("answers")]
    public List<EvaluateAnswerDto> Answers { get; set; } = new();
}

public class EvaluateAnswerDto
{
    [JsonPropertyName("question_id")]
    public int QuestionId { get; set; }

    [JsonPropertyName("student_answer")]
    public string StudentAnswer { get; set; } = string.Empty;
}