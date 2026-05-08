using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.AI;
public class AiQuestionsResponseDto
{
    public string Track { get; set; } = null!;
    public List<AiQuestionDto> Questions { get; set; } = new();
}
