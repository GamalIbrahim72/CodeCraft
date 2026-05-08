using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.AI;
public class AiQuestionDto
{
    public string Question_Id { get; set; } = null!;
    public string Question_Text { get; set; } = null!;
    public string Difficulty_Level { get; set; } = null!;
    public string Topic_Area { get; set; } = null!;
    public string Track { get; set; } = null!;
}
