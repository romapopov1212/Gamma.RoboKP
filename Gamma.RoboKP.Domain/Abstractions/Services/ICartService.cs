using Gamma.RoboKP.Domain.Entities;

namespace Gamma.RoboKP.Domain.Abstractions.Services;

public interface ICartService
{
    Task<List<CartEntity>> GetAll();
    Task<List<ProductEntity>?> GetProducts(long userId);
    Task<long?> AddProduct(long userId, long productId);
    Task<long?> RemoveProduct(long userId, long productId);
    Task<bool> Flush(long userId);
    Task<decimal?> GetTotalCost(long userId);
}