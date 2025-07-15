using Gamma.RoboKP.Domain.Entities;
using Gamma.RoboKP.Domain.ValueObject;
using Microsoft.AspNetCore.Identity;

namespace Gamma.RoboKP.Domain.Abstractions.Services;

public interface IUserService
{
    Task<User?> GetUserByEmail(string email);
    Task<string?> GetUserRole(long id);
    Task<(bool, bool)?> SetUserRole(long id, string role);
    Task<List<User>> GetAllUsers();
    Task<(bool, bool)?> SetStatus(long id, string status);
    Task<string> GetUserStatus(long id);
    Task<bool> UpdateUser(long id, string? firstName = null, string? surName = null, string? lastName = null, string? email = null);
    Task<bool> DeleteUser(long id);
    Task<User?> GetUserById(long id);
    Task<string?> GeneratePasswordResetTokenAsync(string email);
    Task<IdentityResult?> ResetPassword(string email, string token, string newPassword);
    Task<bool> SetCompanyInfo(Company company, long userId);
    (string Email, string ResetToken)? DecodePasswordResetToken(string token);
}