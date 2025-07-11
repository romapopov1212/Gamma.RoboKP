using Gamma.RoboKP.Domain.Abstractions.Repositories;
using Gamma.RoboKP.Domain.Entities;
using Gamma.RoboKP.Infrastructure.Context;
using Gamma.RoboKP.Infrastructure.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.RoboKP.Infrastructure.Repositories;

public class CartRepository([FromKeyedServices("RepositoryMapper")] IMapper mapper, RoboKpDbContext context) : ICartRepository
{
    public async Task<List<CartEntity>> GetAll()
    {
        return await context.Carts
            .AsNoTracking()
            .Include(c => c.CartProducts)
            .Select(c => new CartEntity(c.UserId)
            {
                Products = c.CartProducts.Select(cp => new ProductEntity
                {
                    Id = cp.ProductId 
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<CartEntity?> Get(long userId)
    {
        var result = await context.Carts.FindAsync(userId);
        return result != null ? mapper.Map<CartEntity>(result) : new CartEntity(userId);
    }

    public async Task<long?> AddProduct(long userId, long productId)
    {
        var product = await context.Products.FindAsync(productId);
        if (product == null) return null;
        
        var cart = await context.Carts.FindAsync(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId, CartProducts = new List<CartProduct>() };
            await context.Carts.AddAsync(cart);
        }
        
        var cartProduct = new CartProduct
            { ProductId = productId, Product = product, Cart = cart, CartId = cart.UserId };
        await context.CartProducts.AddAsync(cartProduct);

        return await context.SaveChangesAsync() > 0 ? userId : null;
    }

    public async Task<bool> Flush(long userId)
    {
        var cart = await context.Carts.FindAsync(userId);
        if (cart == null) return false;
        
        context.Carts.Remove(cart);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<long?> RemoveProduct(long userId, long productId)
    {
        var cart = await context.Carts.FindAsync(userId);
        if (cart == null) return null;
        
        var cartProducts = context.CartProducts
            .Where(p => p.CartId == userId && p.ProductId == productId).ToList();

        context.CartProducts.RemoveRange(cartProducts);
        
        return await context.SaveChangesAsync() > 0 ? userId : null;
    }

    public async Task<List<ProductEntity>?> GetProducts(long userId)
    {
        var cart = await context.Carts.FindAsync(userId);
        if (cart == null) return null;
        
        var cartProducts = await context.CartProducts
            .Where(p => p.CartId == userId).ToListAsync();

        var products = cartProducts.Select(cp => context.Products.Find(cp.ProductId)).ToList();

        return mapper.Map<List<ProductEntity>>(products);;
    }
}