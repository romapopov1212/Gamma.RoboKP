using System.ComponentModel.DataAnnotations;

namespace Gamma.RoboKP.Infrastructure.Models;

public class Product
{
    public long Id { get; set; }
    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    [Required]
    public decimal? Price { get; set; }
    [Required]
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;
    public long CategoryId { get; set; }
    public long SubCategoryId { get; set; }
    public required List<CartProduct> CartProducts { get; set; }
    public required Category Category { get; set; }
    public required SubCategory SubCategory { get; set; }
}