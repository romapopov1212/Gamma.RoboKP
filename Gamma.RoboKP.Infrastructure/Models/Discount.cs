using System.ComponentModel.DataAnnotations;

namespace Gamma.RoboKP.Infrastructure.Models;

public class Discount
{
    public Discount(string status, long percent)
    {
        Status = status;
        Percent = percent;
    }
    [Key]
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    [Required]
    [Range(0, 100)]
    public long Percent { get; set; }
}