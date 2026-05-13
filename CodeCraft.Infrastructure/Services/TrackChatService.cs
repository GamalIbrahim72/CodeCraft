using CodeCraft.Application.Common.Exceptions;
using CodeCraft.Application.DTOs.Community;
using CodeCraft.Infrastructure.Persistence;

namespace CodeCraft.Infrastructure.Services;

public class TrackChatService : ITrackChatService
{
    private readonly AppDbContext _context;
    private readonly IUserTrackRepository _userTrackRepository;

    public TrackChatService(AppDbContext context, IUserTrackRepository userTrackRepository)
    {
        _context = context;
        _userTrackRepository = userTrackRepository;
    }

    public async Task<TrackMessageDto> SendMessageAsync(int userId, SendTrackMessageDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Content))
            throw new BadRequestException("Message content is required");

        var trackExists = await _context.Tracks.AnyAsync(t => t.Id == dto.TrackId);

        if (!trackExists)
            throw new KeyNotFoundException("Track not found");

        var isEnrolled = await _userTrackRepository
            .IsUserEnrolledInTrackAsync(userId, dto.TrackId);

        if (!isEnrolled)
            throw new UnauthorizedException("You are not enrolled in this track");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
            throw new KeyNotFoundException("User not found");

        var message = new TrackChatMessage
        {
            Content = dto.Content.Trim(),
            SentAt = DateTime.UtcNow,
            SenderId = userId,
            TrackId = dto.TrackId
        };

        _context.TrackChatMessages.Add(message);

        await _context.SaveChangesAsync();

        return new TrackMessageDto
        {
            Id = message.Id,
            Content = message.Content,
            SentAt = message.SentAt,
            SenderId = userId,
            SenderName = user.FirstName + " " + user.LastName,
            SenderImageUrl = user.ProfileImageUrl,
            TrackId = message.TrackId
        };
    }

    public async Task<IEnumerable<TrackMessageDto>> GetMessagesByTrackAsync(int trackId, int userId)
    {
        var trackExists = await _context.Tracks.AnyAsync(t => t.Id == trackId);

        if (!trackExists)
            throw new KeyNotFoundException("Track not found");

        var isEnrolled = await _userTrackRepository
            .IsUserEnrolledInTrackAsync(userId, trackId);

        if (!isEnrolled)
            throw new UnauthorizedException("You are not enrolled in this track");

        var messages = await _context.TrackChatMessages
            .Where(m => m.TrackId == trackId)
            .Include(m => m.Sender)
            .OrderBy(m => m.SentAt)
            .Select(m => new TrackMessageDto
            {
                Id = m.Id,
                Content = m.Content,
                SentAt = m.SentAt,
                SenderId = m.SenderId,
                SenderName = m.Sender.FirstName + " " + m.Sender.LastName,
                SenderImageUrl = m.Sender.ProfileImageUrl,
                TrackId = m.TrackId
            })
            .ToListAsync();

        return messages;
    }
}