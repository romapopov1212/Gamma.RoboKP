namespace Gamma.RoboKP.Domain.Entities;

public class CartEntity
{
    public CartEntity() {}

    public CartEntity(long userId)
    {
        UserId = userId;
        Products = new List<ProductEntity>();
    }
    
    public long UserId { get; set; }
    public List<ProductEntity> Products { get; set; }
}