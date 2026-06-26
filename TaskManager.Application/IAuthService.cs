using TaskManager.Domain;
namespace TaskManager.Application;

public interface IAuthService
{
    Task<User> RegisterUser(string email, String password);

    Task<string> LoginUser(string email, String password);
}