using System.ComponentModel.DataAnnotations;

namespace Gamma.RoboKP.Models.User;

public class UserToUpdate
{
    public string? EmailToSearch {get; set;}
    public string? FirstName { get; set; }
    public string? SurName { get; set; }
    public string? LastName { get; set; }
    [EmailAddress(ErrorMessage = "Неверный формат почты")]
    public string? Email { get; set; }
    public string? UserName { get; set; }
}