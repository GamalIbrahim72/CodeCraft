using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.DTOs.Profile;
public class UserProfileResponseDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Level { get; set; }
    public List<UserTrackProfileDto> Tracks { get; set; } = new();
}

public class UserTrackProfileDto
{
    public int TrackId { get; set; }
    public string TrackName { get; set; } = string.Empty;
}