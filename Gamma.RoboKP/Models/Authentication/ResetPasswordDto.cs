using System.ComponentModel.DataAnnotations;

namespace Gamma.RoboKP.Models.Authentication;

public class ResetPasswordDto
{
    [Required(ErrorMessage = "Поле 'пароль' обязательно для заполнения")]
    public string? Password { get; set; }
    
    // [Required(ErrorMessage = "Поле 'подтвердите пароль' обязятельно для заполениня")]
    // [Compare("Password", ErrorMessage = "Passwords do not match")]
    // public string? ConfirmPassword { get; set; }
    
    [Required(ErrorMessage = "Доступ запрещен")]
    public string? Token { get; set; }
}