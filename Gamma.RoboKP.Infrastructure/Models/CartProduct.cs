namespace Gamma.RoboKP.Infrastructure.Models;

public class CartProduct
{
    public required long ProductId { get; set; }
    public required Product Product { get; set; }
    public required string CartId { get; set; }
    public required Cart Cart { get; set; }
}