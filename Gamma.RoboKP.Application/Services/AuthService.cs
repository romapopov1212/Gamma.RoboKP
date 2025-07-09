using Gamma.RoboKP.Domain.Abstractions.Auth;
using Gamma.RoboKP.Domain.Abstractions.Repositories;
using Gamma.RoboKP.Domain.Abstractions.Services;
using Gamma.RoboKP.Domain.Entities;
using Gamma.RoboKP.Domain.Enums;
using Gamma.RoboKP.Domain.Exceptions;
//using Gamma.RoboKP.Domain.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Exception = System.Exception;

namespace Gamma.RoboKP.Application.Services;
public class AuthService(ITokenRepository refreshTokenRepository,
    IRefreshTokenService refreshTokenService,
    ITokenService tokenService,
    IUserRepository userRepository,
    IMailService mailService) : IAuthService
{
    public async Task<User> Register(User userRegister, string password)
    {
        var existingUser = await userRepository.FindByEmailAsync(userRegister.Email);
        
        if (existingUser != null) 
        {
            throw new NotValidUserException(
                userRegister,
                new List<IdentityError>() { new IdentityError() {
                    Description="Пользователь с такой почтой уже существует",
                    Code="Email Duplicate"} });
        }
        
        var createUserResult = await userRepository.AddAsync(userRegister, password);
        if (createUserResult)
        {
            var user = await userRepository.FindByEmailAsync(userRegister.Email);

            if (user == null)
            {
                throw new Exception("Что то пошло не так..."); // пока хз
            }
            
            var result = await userRepository.AddToRole(user, UserRole.ManagerPartner.ToString());

            var userRoleString = await userRepository.GetRole(user);
            var userRole = (UserRole)Enum.Parse(typeof(UserRole), userRoleString!, ignoreCase: true);
            
            user.SetRole(userRole);
            
            if (result.Succeeded)
            {
                var confirmationCode =  mailService.GenerateConfirmationCode();
                user.SetVerifyCode(confirmationCode);
                await userRepository.UpdateAsync(user);
                
                var mailData = new MailData(
                    user.Email,
                    user.Email,
                    "Подтверждение регистрации",
                    $"Ваш код подтверждения: {confirmationCode}");
        
                mailService.SendMail(mailData);
                return user;
            }
    
            throw new Exception($"Errors: {string.Join(";", result.Errors
                .Select(x => $"{x.Code} {x.Description}"))}");
        }
        throw new Exception($"Регистрация не удалась: ");
    }
    
    
    public async Task<User> Login(string email, string password)
    {
        var user = await userRepository.FindByEmailAsync(email);
        
        if (user == null)
        {
            throw new EntityNotFoundException(
                new List<IdentityError>{new IdentityError()
                {
                  Description  = $"Пользователь с почтой {email} не найден",
                  Code = "Email not found" } });
        }
        
        var checkPasswordResult = await userRepository.CheckPassword(user, password);
        
        if (checkPasswordResult)
        {
            var userRole = await userRepository.GetRole(user);

            if (userRole == null)
            {
                throw new EntityNotFoundException(
                    new List<IdentityError>{new IdentityError()
                    {
                        Description  = "Ошибка. Пользователю не присвоена роль",
                        Code = "Exception. User role not found." } });
            }

            if (!user.EmailConfirmed)
            {
                var confirmationCode =  mailService.GenerateConfirmationCode();
                user.SetVerifyCode(confirmationCode);
                await userRepository.UpdateAsync(user);
                
                var mailData = new MailData(
                    user.Email,
                    user.Email,
                    "Подтверждение регистрации",
                    $"Ваш код подтверждения: {confirmationCode}");
        
                mailService.SendMail(mailData);
            }
            
            return user;
        }
    
        throw new PasswordFailedException(
            new List<IdentityError>{new IdentityError()
            {
                Description = "Неверный пароль",
                Code = "Invalid Password"} });
    }
    
    public async Task<User?> RefreshAccessToken(string refreshToken)
    {
        var hashToken = refreshTokenService.HashToken(refreshToken);
        
        var refreshTokenEntity = await refreshTokenRepository.GetByHashToken(hashToken);
    
        if (refreshTokenEntity == null || refreshTokenEntity.ExpiresAt < DateTime.Now)
        {
            return null;
        }
        
        var user = await userRepository.FindByIdAsync(refreshTokenEntity.UserId);
        if (user == null)
        {
            return null;
        }
        
        var userRole = await userRepository.GetRole(user);

        if (userRole == null)
        {
            throw new EntityNotFoundException(
                new List<IdentityError>{new IdentityError()
                {
                    Description  = "Ошибка. Пользователю не присвоена роль",
                    Code = "Exception. User role not found." } });
        }
        
        return user;
    }

    public async Task<bool> SendCodeAgain(string email)
    {
        var user = await userRepository.FindByEmailAsync(email);
        
        if (user == null)
        {
            return false;
        }
        
        var confirmationCode =  mailService.GenerateConfirmationCode();
        user.SetVerifyCode(confirmationCode);
        await userRepository.UpdateAsync(user);
                
        var mailData = new MailData(
            user.Email,
            user.Email,
            "Подтверждение регистрации",
            $"Ваш код подтверждения: {confirmationCode}");
         
        var result =mailService.SendMail(mailData);
        return result;
    }
}