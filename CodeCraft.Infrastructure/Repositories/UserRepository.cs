
using CodeCraft.Application.Interfaces.Repositories;
using CodeCraft.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Infrastructure.Repositories;
public class UserRepository:GenericRepository<User> ,IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context):base(context)
    {
        _context = context;
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
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
