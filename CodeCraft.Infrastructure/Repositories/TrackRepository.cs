using CodeCraft.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.Repositories;
public class TrackRepository: GenericRepository<Track>,ITrackRepository
{
    private readonly AppDbContext _context;

    public TrackRepository(AppDbContext context):base(context)
    {
        _context = context;
    }

  
}
