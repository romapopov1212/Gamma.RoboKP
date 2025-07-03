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

    public async Task<List<ProductEntity>?> GetProducts(string userId)
    {
        var products = await cartRepository.GetProducts(userId);
        return products != null ? products : null;
    }

    public async Task<string> AddProduct(string userId, long productId)
    {
        return await cartRepository.AddProduct(userId, productId);
    }

    public async Task<bool> Flush(string userId)
    {
        return await cartRepository.Flush(userId);
    }

    public async Task<string> RemoveProduct(string userId, long productId)
    {
        return await cartRepository.RemoveProduct(userId, productId);
    }

    public async Task<decimal?> GetTotalCost(string userId)
    {
        var cart = await cartRepository.Get(userId);
        if (cart == null) return null;
        return cart.Products.Sum(p => p.Price);
    }
}