using Microsoft.EntityFrameworkCore;
using TaskManager.Domain;

namespace TaskManager.Application;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;

    public AuthService(ApplicationDbContext context)
    {
        _context = context;
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
        return "";
    }
}