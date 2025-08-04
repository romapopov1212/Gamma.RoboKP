using Gamma.RoboKP.Domain.Entities;
using Gamma.RoboKP.Domain.ValueObject;
using Microsoft.AspNetCore.Identity;

namespace Gamma.RoboKP.Domain.Abstractions.Repositories;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByIdAsync(long? id);
    Task<bool> AddAsync(User user, string password);
    Task<IdentityResult> UpdateAsync(User user);
    Task<IdentityResult> AddToRole(User user, string role);
    Task<bool> CheckPassword(User user, string password);
    Task<string?> GetRole(User user);
    Task<List<User>> GetAll();
    Task RemoveFromRole(User user, string role);
    Task<IdentityResult> Delete(User user);
    Task<string?> GeneratePasswordResetTokenAsync(string email);
    Task<IdentityResult?> ResetPassword(string email, string token, string newPassword);
    Task<bool> SetCompanyInfo(Company company, long userId);
    Task<IdentityResult> SetConfirmCode(long userId, string code);
    Task<IdentityResult> ConfirmEmail(long userId);
}