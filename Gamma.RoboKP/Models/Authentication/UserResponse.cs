using Gamma.RoboKP.Domain.ValueObject;

namespace Gamma.RoboKP.Models.Authentication;

public class UserResponse{
    public long Id { get; set; }
    public string FirstName { get; set; }
    public string SurName { get; set; }
    public string LastName { get; set; }
    public string Role { get; set; }
    public string Status { get; set; }
    public string Email {get; set;}
    
    //public Company Company { get; set; }
    public string UserName { get; set; }
    //public string Token {get; set;}
    //public string RefreshToken {get; set;}
    };