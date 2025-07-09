using Gamma.RoboKP.Domain.Entities;

namespace Gamma.RoboKP.Domain.Abstractions.Auth;

public interface IAuthService
{
    Task<User> Register(User userRegister, string password);
    Task<User> Login(string email, string password);
    
    Task<User?> RefreshAccessToken(string refreshToken);

    Task<bool> SendCodeAgain(string email);
}