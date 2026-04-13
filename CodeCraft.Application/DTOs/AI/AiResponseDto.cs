using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.AI;
public class AiResponseDto
{
    public string Question { get; set; }= string.Empty;
    public string Level { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}
