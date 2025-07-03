using Gamma.RoboKP.Domain.Abstractions.Repositories;
using Gamma.RoboKP.Domain.Abstractions.Services;
using Gamma.RoboKP.Domain.Entities;
using Gamma.RoboKP.Domain.Enums;
using Gamma.RoboKP.Domain.Exceptions;
using Gamma.RoboKP.Domain.ValueObject;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;


namespace Gamma.RoboKP.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
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
    
    public async Task SetUserRole(long id, string role)
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
        var currentUserRoles = await userRepository.GetRole(user);
        
        if (currentUserRoles == null)
        {
            throw new EntityNotFoundException(
                new List<IdentityError>{new IdentityError()
                {
                    Description  = "Ошибка. Пользователю не присвоена роль",
                    Code = "Exception. User role not found." } });
        }
        await userRepository.RemoveFromRole(user, currentUserRoles);
        
        var isSuccessful = await userRepository.AddToRole(user, role);
        if (!isSuccessful.Succeeded)
        {
            var errors = string.Join("; ", isSuccessful.Errors.Select(e => $"{e.Code}: {e.Description}"));
            throw new Exception($"Ошибка при добавлении роли: {errors}");
        }
    }
    
    public async Task<bool> SetStatus(long id, string status)
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
        
        if (!Enum.TryParse<UserStatus>(status, ignoreCase: true, out var parsedStatus))
        {
            throw new ArgumentException($"Недопустимый статус: {status}", nameof(status));
        }
        
        user.SetStatus(parsedStatus);
        
        var result = await userRepository.UpdateAsync(user);
        
        return result.Succeeded;
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
    
    public async Task<bool> UpdateUser(long id,
        string? firstName = null, 
        string? surName=null,
        string? lastName=null,
        string? email=null)
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
        return await userRepository.FindByIdAsync(id);;
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
}
