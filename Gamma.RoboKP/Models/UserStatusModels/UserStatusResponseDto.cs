using System.ComponentModel.DataAnnotations;

namespace Gamma.RoboKP.Models.UserStatusModels;

public record UserStatusResponseDto(
    [Required]
    string Status,
    [Required]
    long Percent
    );