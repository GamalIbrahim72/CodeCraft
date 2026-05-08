using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.AI;
public class ExamQuestionResponseDto
{
    public string SessionId { get; set; } = null!;
    public int QuestionNumber { get; set; }
    public int TotalQuestions { get; set; }
    public AiQuestionDto Question { get; set; } = null!;
    public bool IsFinished { get; set; }
}
