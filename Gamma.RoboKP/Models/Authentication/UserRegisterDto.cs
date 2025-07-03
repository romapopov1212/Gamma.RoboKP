using System.ComponentModel.DataAnnotations;
using Gamma.RoboKP.Domain.ValueObject;

namespace Gamma.RoboKP.Models.Authentication;

public record UserRegisterDto(
    [Required(ErrorMessage = "Поле 'имя' обязательно для заполнения")]
    string FirstName,
    
    [Required(ErrorMessage = "Поле 'фамилия' обязательно для заполнения")]
    string SurName,
    string LastName,
    
    [Required(ErrorMessage = "Поле 'почта' обязательно для заполнения")]
    [EmailAddress(ErrorMessage = "Некорректный формат почты")]
    string Email,
    
    [Required(ErrorMessage = "Поле 'пароль' обязательно для заполнения")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Минимальная длина пароля - 8 символов")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Пароль должен содержать буквы разного регистра и цифры")]
    string Password
    
   // [Required(ErrorMessage = "Компания обязательна")]
   // Company Company
    );