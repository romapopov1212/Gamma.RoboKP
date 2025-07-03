using Gamma.RoboKP.Domain.Abstractions.Repositories;
using Gamma.RoboKP.Domain.Entities;
using Gamma.RoboKP.Infrastructure.Context;
using Gamma.RoboKP.Infrastructure.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.RoboKP.Infrastructure.Repositories;

public class DiscountRepository(
    [FromKeyedServices("RepositoryMapper")] IMapper mapper,
    RoboKpDbContext context
    ) : IDiscountRepository
{
    public async Task<string?> Create(string status, long percent)
    {
        var discount = new Discount(status, percent);
        await context.Discounts.AddAsync(discount);
        return await context.SaveChangesAsync() > 0 ? status : null;
    }
    
    public async Task<string?> Update(string status, long percent)
    {
        var discount = await context.Discounts.FindAsync(status);
        if (discount == null) return null;
        discount.Percent = percent;
        context.Discounts.Update(discount);
        return await context.SaveChangesAsync() > 0 ? status : null;
    }

    public async Task<DiscountEntity?> Get(string status)
    {
        var discount = await context.Discounts.FindAsync(status);
        return discount != null ? mapper.Map<DiscountEntity>(discount) : null;
    }
    
    public async Task<List<DiscountEntity>> GetAll()
    {
        var discounts = await context.Discounts.ToListAsync();
        return mapper.Map<List<DiscountEntity>>(discounts);
    }

    public async Task<bool> Delete(string status)
    {
        var discount = await context.Discounts.FindAsync(status);
        if (discount == null) return false;
        context.Discounts.Remove(discount);
        return await context.SaveChangesAsync() > 0;
    }
}