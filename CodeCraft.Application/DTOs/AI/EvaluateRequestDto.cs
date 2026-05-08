using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.AI;
public class EvaluateRequestDto
{
    [JsonPropertyName("answers")]
    public List<AnswerEvaluationDto> Answers { get; set; } = new();

}
