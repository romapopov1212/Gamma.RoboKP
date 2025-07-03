using System.ComponentModel.DataAnnotations;

namespace Gamma.RoboKP.Models.Cart;

public record CartResponseDto(
    [Required]
    long UserId,
    [Required]
    List<long> ProductIds
    );