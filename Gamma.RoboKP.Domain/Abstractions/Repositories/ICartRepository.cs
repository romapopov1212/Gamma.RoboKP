using Gamma.RoboKP.Domain.Entities;

namespace Gamma.RoboKP.Domain.Abstractions.Repositories;

public interface ICartRepository
{
    Task<List<CartEntity>> GetAll();
    Task<CartEntity> Get(string userId);
    Task<string> AddProduct(string userId, long productId);
    Task<string> RemoveProduct(string userId, long productId);
    Task<bool> Flush(string userId);
    Task<List<ProductEntity>?> GetProducts(string userId);
}