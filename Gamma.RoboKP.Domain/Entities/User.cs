using Gamma.RoboKP.Domain.Enums;
using Gamma.RoboKP.Domain.ValueObject;

namespace Gamma.RoboKP.Domain.Entities;

public class User : BaseEntity<long>
{
    public User() { }
    
    private User(string firstName, string surName, string lastName, UserStatus status, UserRole role, Company company, string email)
    {
        FirstName = firstName;
        SurName = surName;
        LastName = lastName;
        Status = status;
        Role = role;
        Company = company;
        Email = email;
    }
    
    public string FirstName { get; private set; } 
    public string SurName { get; private set; }
    public string LastName { get; private set; }
    public UserStatus Status { get; private set; }
    public UserRole Role { get; private set; }
    public Company Company { get; private set; }
    public string Email { get; private set; }
    public bool EmailConfirmed { get; private set; }
    public string? VerifyCode { get; private set; }

    public void SetEmail(string email)
    {
        if(string.IsNullOrEmpty(email))
            throw new ArgumentException("Почта обязательна");

        if (!IsValidEmail(email))
            throw new ArgumentException("Не верный формат почты");
        
        if (email == Email) return;
        Email = email;
        EmailConfirmed = false;
    }

    public void SetVerifyCode(string? verifyCode)
    {
        VerifyCode = verifyCode;
    }

    public void ConfirmEmail()
    {
        EmailConfirmed = true;
    }

    public void SetStatus(UserStatus status)
    {
        Status = status;
    }

    public void SetFirstName(string firstName)
    {
        FirstName = firstName;
    }

    public void SetSurName(string surName)
    {
        SurName = surName;
    }

    public void SetLastName(string lastName)
    {
        LastName = lastName;
    }

    public void SetRole(UserRole role)
    {
        Role = role;
    }

    public static User Create(string firstName,
        string surName,
        string lastName,
        UserStatus status,
        UserRole role,
        Company company,
        string email)
    {
        return new User(firstName, surName, lastName, status, role, company, email);
    }

    public bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}//TODO:многие ко многим с категорией(создать таблицу категории)