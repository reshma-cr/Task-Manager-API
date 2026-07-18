using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Domain;

namespace TaskManager.Application;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<User> RegisterUser(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if(user != null)
        {
            throw new Exception("User email already exists");
        }
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        User newUser = new User()
        {
            Id = Guid.NewGuid(),
            Email = email,
            PasswordHash = hashedPassword
        };
        await _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();
        return newUser;
    }

    public async Task<string> LoginUser(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if(user == null)
        {
            throw new Exception("invalid email or password");
        }
        var match = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        if (!match)
        {
            throw new Exception("invalid email or password");
        }
        var secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? _configuration["Jwt:Secret"];
        var issuer = _configuration["Jwt:Issuer"];
        var expiry = _configuration["Jwt:ExpiryInMinutes"];
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = issuer,
            Expires = DateTime.UtcNow.AddMinutes(double.Parse(expiry)),
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}