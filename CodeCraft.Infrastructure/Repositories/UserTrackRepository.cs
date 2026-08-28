using CodeCraft.Application.DTOs.Admin;
using CodeCraft.Infrastructure.Persistence;

namespace CodeCraft.Infrastructure.Repositories;
public class UserTrackRepository: IUserTrackRepository
{
    private readonly AppDbContext _context;

    public UserTrackRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsUserEnrolledAsync(int userId, int trackId)
    {
        return await _context.UserTracks
            .AnyAsync(x => x.UserId == userId && x.TrackId == trackId);
    }

    public async Task EnrollUserAsync(int userId, int trackId)
    {
        if (await IsUserEnrolledAsync(userId, trackId))
            throw new Exception("Already enrolled");

        var userTrack = new UserTrack
        {
            UserId = userId,
            TrackId = trackId
        };

        await _context.UserTracks.AddAsync(userTrack);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Track>> GetUserTracksAsync(int userId)
    {
        return await _context.UserTracks
            .Where(x => x.UserId == userId)
            .Select(x => x.Track)
            .ToListAsync();
    }
    public async Task<List<User>> GetEnrolledUsersAsync()
    {
        return await _context.UserTracks
            .Include(x => x.User)
            .Select(x => x.User)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> IsUserEnrolledInTrackAsync(int userId, int trackId)
    {
        return await _context.UserTracks
            .AnyAsync(ut => ut.UserId == userId && ut.TrackId == trackId);
    }

    public async Task<List<UserTrack>> GetByUserIdAsync(int userId)
    {
        return await _context.UserTracks
            .Include(x => x.Track)
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }

    public async Task<List<AdminTrackStatsDto>> GetTrackStatsAsync()
    {
        return await _context.UserTracks
            .Include(ut => ut.Track)
            .Include(ut => ut.User)
            .GroupBy(ut => new
            {
                ut.TrackId,
                TrackName = ut.Track.Name
            })
            .Select(g => new AdminTrackStatsDto
            {
                TrackId = g.Key.TrackId,
                TrackName = g.Key.TrackName,
                TotalUsers = g.Select(x => x.UserId).Distinct().Count(),
                Levels = g
                    .GroupBy(x => x.User.Level ?? "Unknown")
                    .Select(levelGroup => new AdminLevelStatsDto
                    {
                        Level = levelGroup.Key,
                        UsersCount = levelGroup.Select(x => x.UserId).Distinct().Count()
                    })
                    .ToList()
            })
            .ToListAsync();
    }
}
