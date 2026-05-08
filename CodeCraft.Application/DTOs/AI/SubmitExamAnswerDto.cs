using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.AI;
public class SubmitExamAnswerDto
{
    public string SessionId { get; set; } = null!;
    public string StudentAnswer { get; set; } = null!;
}
