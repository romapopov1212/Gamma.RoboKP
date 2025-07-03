using System.ComponentModel.DataAnnotations;

namespace Gamma.RoboKP.Infrastructure.Models;

public class Cart
{
    [Key]
    public string UserId { get; set; }
    public required List<CartProduct> CartProducts { get; set; } = new();
}