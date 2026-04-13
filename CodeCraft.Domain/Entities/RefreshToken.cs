using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeCraft.Domain.Entities;
public class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; } = false;
}
