using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.AI;
public class AnswerEvaluationDto
{
    [JsonPropertyName("question_id")]
    public string Question_Id { get; set; } = null!;

    [JsonPropertyName("question_text")]
    public string Question_Text { get; set; } = null!;

    [JsonPropertyName("difficulty_level")]
    public string Difficulty_Level { get; set; } = null!;

    [JsonPropertyName("topic_area")]
    public string Topic_Area { get; set; } = null!;

    [JsonPropertyName("track")]
    public string Track { get; set; } = null!;

    [JsonPropertyName("student_answer")]
    public string Student_Answer { get; set; } = null!;
}
