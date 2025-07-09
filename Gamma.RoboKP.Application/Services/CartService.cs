using Gamma.RoboKP.Domain.Abstractions.Repositories;
using Gamma.RoboKP.Domain.Abstractions.Services;
using Gamma.RoboKP.Domain.Entities;

namespace Gamma.RoboKP.Application.Services;

public class CartService(ICartRepository cartRepository) : ICartService
{
     public async Task<List<CartEntity>> GetAll()
    {
        return await cartRepository.GetAll();
    }

    public async Task<List<ProductEntity>?> GetProducts(long userId)
    {
        var products = await cartRepository.GetProducts(userId);
        return products;
    }

    public async Task<long?> AddProduct(long userId, long productId)
    {
        return await cartRepository.AddProduct(userId, productId);
    }

    public async Task<bool> Flush(long userId)
    {
        return await cartRepository.Flush(userId);
    }

    public async Task<long?> RemoveProduct(long userId, long productId)
    {
        return await cartRepository.RemoveProduct(userId, productId);
    }

    public async Task<decimal?> GetTotalCost(long userId)
    {
        var cart = await cartRepository.Get(userId);
        return cart?.Products.Sum(p => p.Price);
    }
}