using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gamma.RoboKP.Domain.Abstractions.Repositories;
using Gamma.RoboKP.Domain.Entities;
using Gamma.RoboKP.Domain.Enums;
using Gamma.RoboKP.Domain.Exceptions;
using Gamma.RoboKP.Domain.Options;
using Gamma.RoboKP.Domain.ValueObject;
using Gamma.RoboKP.Infrastructure.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Gamma.RoboKP.Infrastructure.Repositories;

public class UserRepository(UserManager<AppUser> userManager,
    [FromKeyedServices("RepositoryMapper")] IMapper mapper,
    IOptions<AuthOptions> authOptions) : IUserRepository
{
    
    private readonly AuthOptions _authOptions = authOptions.Value;
    public async Task<User?> FindByEmailAsync(string email)
    {
        var appUser = await userManager.FindByEmailAsync(email);
        
        if (appUser == null) return null;
        
        var entity = mapper.Map<AppUser, User>(appUser);
        
        var userRole = await userManager.GetRolesAsync(appUser);
        
        Enum.TryParse<UserRole>(userRole.FirstOrDefault(), true, out var role);
        entity.SetRole(role);
        
        return entity;
    }

    public async Task<User?> FindByIdAsync(long? id)
    {
        var appUser = await userManager.FindByIdAsync(id.ToString()!);
        if (appUser == null) return null;
        
        var entity = mapper.Map<AppUser, User>(appUser);
        
        var userRole = await userManager.GetRolesAsync(appUser);
        
        Enum.TryParse<UserRole>(userRole.FirstOrDefault(), true, out var role);
        entity.SetRole(role);
        
        return entity;
    }

    public async Task<bool> AddAsync(User user, string password)
    {
        var appUser = mapper.Map<User, AppUser>(user);
        appUser.UserName = user.Email;
        var result = await userManager.CreateAsync(appUser, password);
        
        return result.Succeeded;
    }

    public async Task<IdentityResult> AddToRole(User user, string role)
    {
        var appUser = await userManager.FindByIdAsync(user.Id.ToString());

        if (appUser == null)
            return IdentityResult.Failed();

        var result = await userManager.AddToRoleAsync(appUser, role);
        
        return result;
    }

    public async Task<bool> CheckPassword(User user, string password)
    {
        var userApp = await userManager.FindByIdAsync(user.Id.ToString());
        if (userApp == null) return false;
        
        var result = await userManager.CheckPasswordAsync(userApp, password);
        
        return result;
    }

    public async Task<string?> GetRole(User user)
    {
        var userApp = await userManager.FindByIdAsync(user.Id.ToString());
        
        if (userApp == null) return null;
        
        var role = await userManager.GetRolesAsync(userApp);
        return role.FirstOrDefault();
    }

    public async Task<List<User>> GetAll()
    {
        var usersApp = await userManager.Users.ToListAsync();
        
        var users = mapper.Map<List<AppUser>, List<User>>(usersApp);

        for (int i = 0; i < users.Count; i++)
        {
            var roles = await userManager.GetRolesAsync(usersApp[i]);
            if (Enum.TryParse<UserRole>(roles.FirstOrDefault(), true, out var role))
            {
                users[i].SetRole(role);
            }
        }

        return users;
    }

    public async Task RemoveFromRole(User user, string role)
    {
        var userApp = await userManager.FindByIdAsync(user.Id.ToString());
        if (userApp == null) throw new InvalidOperationException($"User with ID {user.Id} not found");
        
        await userManager.RemoveFromRoleAsync(userApp, role);
    }

    public async Task<IdentityResult> UpdateAsync(User user)
    {
        
        var appUser = await userManager.FindByIdAsync(user.Id.ToString());

        if (appUser == null)
        {
            throw new EntityNotFoundException(new List<IdentityError>
            {
                new IdentityError
                {
                    Description = $"Пользователь с id {user.Id} не найден",
                    Code = "UserNotFound"
                }
            });
        }
        
        appUser.Email = user.Email;
        appUser.FirstName = user.FirstName;
        appUser.LastName = user.LastName;
        appUser.SurName = user.SurName;
        appUser.Status = user.Status;
        appUser.VerifyCode = user.VerifyCode;
        if (user.EmailConfirmed) appUser.EmailConfirmed = true;
        
        var result = await userManager.UpdateAsync(appUser);
        return result;
    }

    public async Task<IdentityResult> ConfirmEmail(long userId)
    {
        var appUser = await userManager.FindByIdAsync(userId.ToString());
        if (appUser == null)
        {
            throw new EntityNotFoundException(new List<IdentityError>
            {
                new IdentityError
                {
                    Description = $"Пользователь с id {userId} не найден",
                    Code = "UserNotFound"
                }
            });
        }
        appUser.EmailConfirmed = true;
        var result = await userManager.UpdateAsync(appUser);
        return result;
    }

    public async Task<IdentityResult> SetConfirmCode(long userId, string code)
    {
        var appUser = await userManager.FindByIdAsync(userId.ToString());
        if (appUser == null)
        {
            throw new EntityNotFoundException(new List<IdentityError>
            {
                new IdentityError
                {
                    Description = $"Пользователь с id {userId} не найден",
                    Code = "UserNotFound"
                }
            });
        }
        appUser.VerifyCode = code;
        var result = await userManager.UpdateAsync(appUser);
        return result;
    }


    public async Task<IdentityResult> Delete(User user)
    {
        var appUser = await userManager.FindByIdAsync(user.Id.ToString());
        if (appUser == null) throw new InvalidOperationException($"User with ID {user.Id} not found");
        
        var result = await userManager.DeleteAsync(appUser);
        
        return result;
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(string email)
    {
        var appUser = await userManager.FindByEmailAsync(email);
        if (appUser == null) return null;
        
        var resetToken = await userManager.GeneratePasswordResetTokenAsync(appUser);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_authOptions.TokenPrivateKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.Email, email),
                new Claim("reset_token", resetToken),
            ]),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);
        
        return jwtToken;
    }

    public async Task<IdentityResult?> ResetPassword(string email, string token, string newPassword)
    {
        var appUser = await userManager.FindByEmailAsync(email);
        if (appUser == null) return null;
        
        var result = await userManager.ResetPasswordAsync(appUser, token, newPassword);
        
        return result;
    }

    public async Task<bool> SetCompanyInfo(Company company, long userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;
        
        user.Company = company;
        var result = await userManager.UpdateAsync(user);
        
        return result.Succeeded;
    }
}