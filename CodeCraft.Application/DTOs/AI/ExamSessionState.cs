using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.AI;
public class ExamSessionState
{

    public string Track { get; set; } = string.Empty;

    public List<AiQuestionDto> Questions { get; set; } = new();

    public int CurrentIndex { get; set; }

    public List<EvaluateAnswerDto> Answers { get; set; } = new();
}
