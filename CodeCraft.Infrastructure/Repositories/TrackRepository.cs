using CodeCraft.Application.Interfaces.Repositories;
using CodeCraft.Domain.Entities;
using CodeCraft.Infrastructure.Persistence;

namespace CodeCraft.Infrastructure.Repositories;

public class TrackRepository : GenericRepository<Track>, ITrackRepository
{
    public TrackRepository(AppDbContext context) : base(context)
    {
    }
}
