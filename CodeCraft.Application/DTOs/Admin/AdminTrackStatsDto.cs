using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.Admin;
public class AdminTrackStatsDto
{
    public int TrackId { get; set; }
    public string TrackName { get; set; } = string.Empty;
    public int TotalUsers { get; set; }
    public List<AdminLevelStatsDto> Levels { get; set; } = new();
}

public class AdminLevelStatsDto
{
    public string Level { get; set; } = string.Empty;
    public int UsersCount { get; set; }
}