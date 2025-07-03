namespace Gamma.RoboKP.Domain.Entities;

public class CartEntity
{
    public CartEntity() {}

    public CartEntity(string userId)
    {
        UserId = userId;
        Products = new List<ProductEntity>();
    }
    
    public string UserId { get; set; }
    public List<ProductEntity> Products { get; private set; }
}