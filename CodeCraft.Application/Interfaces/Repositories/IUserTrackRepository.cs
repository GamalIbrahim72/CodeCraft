using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Repositories;
public interface IUserTrackRepository
{
    Task EnrollUserAsync(int userId, int trackId);

    Task<bool> IsUserEnrolledAsync(int userId, int trackId);

    Task<List<Track>> GetUserTracksAsync(int userId);
    Task<List<User>> GetEnrolledUsersAsync();
    Task<bool> IsUserEnrolledInTrackAsync(int userId, int trackId);
    Task<List<UserTrack>> GetByUserIdAsync(int userId);
}
