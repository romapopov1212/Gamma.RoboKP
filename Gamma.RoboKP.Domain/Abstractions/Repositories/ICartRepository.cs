using Gamma.RoboKP.Domain.Entities;

namespace Gamma.RoboKP.Domain.Abstractions.Repositories;

public interface ICartRepository
{
    Task<List<CartEntity>> GetAll();
    Task<CartEntity?> Get(long userId);
    Task<long?> AddProduct(long userId, long productId);
    Task<long?> RemoveProduct(long userId, long productId);
    Task<bool> Flush(long userId);
    Task<List<ProductEntity>?> GetProducts(long userId);
}