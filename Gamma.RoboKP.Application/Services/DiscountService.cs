using Gamma.RoboKP.Domain.Abstractions.Repositories;
using Gamma.RoboKP.Domain.Abstractions.Services;
using Gamma.RoboKP.Domain.Entities;

namespace Gamma.RoboKP.Application.Services;

public class DiscountService(IDiscountRepository repository) : IDiscountService
{
    public async Task<string?> Create(string status, long percent)
    {
        return await repository.Create(status, percent);
    }
    
    public async Task<string?> Update(string status, long percent)
    {
        return await repository.Update(status, percent);
    }

    public async Task<DiscountEntity?> Get(string status)
    {
        return await repository.Get(status);
    }
    
    public async Task<List<DiscountEntity>> GetAll()
    {
        return await repository.GetAll();
    }

    public async Task<bool> Delete(string status)
    {
        return await repository.Delete(status);
    }
}