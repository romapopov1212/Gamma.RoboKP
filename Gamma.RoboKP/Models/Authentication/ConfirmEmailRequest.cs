namespace Gamma.RoboKP.Models.Authentication;

public record ConfirmEmailRequest(string Email, string Code);