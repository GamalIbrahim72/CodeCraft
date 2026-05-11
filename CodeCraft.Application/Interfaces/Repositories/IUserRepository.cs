using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Application.Interfaces.Repositories;
public interface IUserRepository:IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);

    Task<User?> GetByResetTokenAsync(string token);
    
    Task<User?> GetByPhoneAsync(string phone);

}
