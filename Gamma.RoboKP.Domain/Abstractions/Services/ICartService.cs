using Gamma.RoboKP.Domain.Entities;

namespace Gamma.RoboKP.Domain.Abstractions.Services;

public interface ICartService
{
    Task<List<CartEntity>> GetAll();
    Task<List<ProductEntity>?> GetProducts(string userId);
    Task<string> AddProduct(string userId, long productId);
    Task<string> RemoveProduct(string userId, long productId);
    Task<bool> Flush(string userId);
    Task<decimal?> GetTotalCost(string userId);
}