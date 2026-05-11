using CodeCraft.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            .Where(x => x.UserId == userId)
            .ToListAsync();
    }
}
