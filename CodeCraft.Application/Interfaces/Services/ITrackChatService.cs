using CodeCraft.Application.DTOs.Community;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Services;
public interface ITrackChatService
{
    Task<TrackMessageDto> SendMessageAsync(int userId, SendTrackMessageDto dto);
    Task<IEnumerable<TrackMessageDto>> GetMessagesByTrackAsync(int trackId, int userId);
}
