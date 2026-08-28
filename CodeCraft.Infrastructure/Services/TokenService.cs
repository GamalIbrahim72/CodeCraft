using System;

namespace CodeCraft.Application.Services;
public class TokenService: ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var keyString = _configuration["Jwt:Key"] ?? "Default_Secret_Key_For_Development_Only_32bytes_Long";
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(keyString)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var durationMinutes = double.TryParse(_configuration["Jwt:DurationInMinutes"], out var d) ? d : 60;

        var token = new JwtSecurityToken(
             issuer: _configuration["Jwt:Issuer"] ?? "CodeCraftAPI",
             audience: _configuration["Jwt:Audience"] ?? "CodeCraftClients",
             claims: claims,
             expires: DateTime.UtcNow.AddMinutes(durationMinutes),
             signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
