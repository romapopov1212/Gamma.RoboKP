using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Gamma.RoboKP.Domain.Abstractions.Repositories;
using Gamma.RoboKP.Domain.Abstractions.Services;
using Gamma.RoboKP.Domain.Entities;
using Gamma.RoboKP.Domain.Enums;
using Gamma.RoboKP.Domain.Exceptions;
using Gamma.RoboKP.Domain.Options;
using Gamma.RoboKP.Domain.ValueObject;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;


namespace Gamma.RoboKP.Application.Services;

public class UserService(
    ILogger<UserService> logger,
    IUserRepository userRepository, IOptions<AuthOptions> authOptions
    ) : IUserService
{
    private readonly AuthOptions _authOptions = authOptions.Value;
    
    public async Task<string?> GetUserRole(long id)
    {
        var user = await userRepository.FindByIdAsync(id);
        if (user == null)
        {
            return null;
        }
        
        var role = await userRepository.GetRole(user);
        
        return role;
    }
    
    public async Task<(bool, bool)?> SetUserRole(long id, string role)
    {
        var user = await userRepository.FindByIdAsync(id);
        if (user == null)
        {
            return (false, true);
        }
        var currentUserRoles = await userRepository.GetRole(user);
        
        if (currentUserRoles == null) // TODO возможно улучшение обработки ошибки
        {
            return null;
        }
        
        await userRepository.RemoveFromRole(user, currentUserRoles);
        
        var isSuccessful = await userRepository.AddToRole(user, role);
        if (!isSuccessful.Succeeded)
        {
            var errors = string.Join("; ", isSuccessful.Errors.Select(e => $"{e.Code}: {e.Description}"));
            logger.LogError("Ошибка при добавлении роли: {}", errors);
            return (true, false);
        }

        return (true, true);
    }
    
    public async Task<(bool, bool)?> SetStatus(long id, string status)
    {
        var user = await userRepository.FindByIdAsync(id);

        if (user == null) return (false, true);
        
        if (!Enum.TryParse<UserStatus>(status, ignoreCase: true, out var parsedStatus))
            return (true, false);
        
        user.SetStatus(parsedStatus);
        
        var result = await userRepository.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => $"{e.Code}: {e.Description}"));
            logger.LogError("Ошибка при обновлении базы данных: {}", errors);
            return null;
        }
        
        return (true, true);
    }
    
    public async Task<string> GetUserStatus(long id)
    {
        var user = await userRepository.FindByIdAsync(id);
        
        if (user == null)
        {
            throw new EntityNotFoundException(
                new List<IdentityError>{new IdentityError()
                {
                    Description  = $"Пользователь с id {id} не найден",
                    Code = "User not found" } });
        }
        return user.Status.ToString();
    }
    
    public async Task<bool> UpdateUser(long? id,
        string? emailToSearch = null,
        string? firstName = null, 
        string? surName=null,
        string? lastName=null,
        string? email=null)
    {
        User? user = new User();
        if (id != null)
        {
            user = await userRepository.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }
        }

        if (emailToSearch != null)
        {
            user = await userRepository.FindByEmailAsync(emailToSearch);
            if (user == null)
            {
                return false;
            }
        }
        
        
        if (firstName != null) user.SetFirstName(firstName);
        
        if(surName != null) user.SetSurName(surName);
        
        if(lastName != null) user.SetLastName(lastName);
        
        if (email != null) user.SetEmail(email);
        
        var result = await userRepository.UpdateAsync(user);
        
        return result.Succeeded;
    }
    
    public async Task<List<User>> GetAllUsers()
    {
        var usersEntity = await userRepository.GetAll();
        
        return usersEntity;
    }
    
    public async Task<bool> DeleteUser(long id)
    {
        var user = await userRepository.FindByIdAsync(id);
        if (user == null) return false;
        
        var result = await userRepository.Delete(user);
        
        return result.Succeeded;
    }
    
    public async Task<User?> GetUserById(long id)
    {
        return await userRepository.FindByIdAsync(id);
    } 
    public async Task<User?> GetUserByEmail(string email)
    {
        var entity = await userRepository.FindByEmailAsync(email);

        return entity;
    }

    public async Task<string?> GeneratePasswordResetTokenAsync(string email)
    {
        return await userRepository.GeneratePasswordResetTokenAsync(email);
    }

    public async Task<IdentityResult?> ResetPassword(string email, string token, string newPassword)
    {
        return await userRepository.ResetPassword(email, token, newPassword);
    }

    public async Task<bool> SetCompanyInfo(Company company, long userId)
    {
        return await userRepository.SetCompanyInfo(company, userId);
    }
    
    public (string Email, string ResetToken)? DecodePasswordResetToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_authOptions.TokenPrivateKey);
    
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);
    
        var jwtToken = (JwtSecurityToken)validatedToken;
        var emailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "email");
        var resetTokenClaim = jwtToken.Claims.First(x => x.Type == "reset_token");

        if (emailClaim == null || resetTokenClaim == null)
        {
            return null;
        }
        
        return (emailClaim.Value, resetTokenClaim.Value);
    }
    
}
