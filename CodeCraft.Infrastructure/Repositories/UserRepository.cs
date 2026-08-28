using CodeCraft.Application.Interfaces.Repositories;
using CodeCraft.Domain.Entities;
using CodeCraft.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CodeCraft.Infrastructure.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetByResetTokenAsync(string token)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.ResetToken == token);
    }
    
    public async Task<User?> GetByPhoneAsync(string phone)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Phone == phone);
    }
}
