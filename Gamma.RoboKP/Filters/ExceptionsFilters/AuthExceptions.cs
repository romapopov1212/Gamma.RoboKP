using Gamma.RoboKP.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gamma.RoboKP.Filters.ExceptionsFilters;

public class AuthExceptions : Attribute, IAsyncExceptionFilter
{
    public Task OnExceptionAsync(ExceptionContext context)
    {
        var exception = context.Exception;
        if (context.Exception is EntityNotFoundException notFound)
        {
            context.Result = new JsonResult(new { message="Введены не валидные данные", errors = notFound.Errors }) 
                { StatusCode= 400 };
        } else if (context.Exception is PasswordFailedException passwordFailed)
        {
            context.Result = new JsonResult(new { message = "Неверный пароль", errors = passwordFailed.Errors }) 
                { StatusCode = 400 };
        } else if (context.Exception is NotValidUserException userException)
        {
            context.Result = new JsonResult(new { message = "Пользователь с такой почтой уже существует", errors = userException.Errors })
                { StatusCode = 409 };
        }else if (exception is System.Exception ex && ex.Message.Contains("Регистрация не удалась:"))
        {
            var errors = new List<string>();

            if (ex.Message.Contains("PasswordRequiresDigit"))
                errors.Add("Пароль должен содержать хотя бы одну цифру(0-9)");
    
            if (ex.Message.Contains("PasswordRequiresUpper"))
                errors.Add("Пароль должен содержать хотя бы одну заглавную букву (A-Z)");

            var isOnlyPasswordErrors = errors.Count > 0;

            var message = isOnlyPasswordErrors 
                ? "Ошибка валидации пароля" 
                : "Регистрация не удалась: неизвестная причина";

            context.Result = new JsonResult(new { message, errors }) { StatusCode = 400 };
        }
        
        return Task.CompletedTask;
    }
}