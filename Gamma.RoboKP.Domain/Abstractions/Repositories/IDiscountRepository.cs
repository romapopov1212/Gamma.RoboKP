using Gamma.RoboKP.Domain.Entities;

namespace Gamma.RoboKP.Domain.Abstractions.Repositories;

public interface IDiscountRepository
{
    Task<string?> Create(string status, long percent);
    Task<string?> Update(string status, long percent);
    Task<DiscountEntity?> Get(string status);
    Task<List<DiscountEntity>> GetAll();
    Task<bool> Delete(string status);
}