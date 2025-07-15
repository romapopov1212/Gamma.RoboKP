using Gamma.RoboKP.Domain.Abstractions.Repositories;
using Gamma.RoboKP.Domain.Abstractions.Services;
using Gamma.RoboKP.Domain.Entities;

namespace Gamma.RoboKP.Application.Services;

public class SubCategoryService(ISubCategoryRepository repository) : ISubCategoryService
{
    public async Task<bool?> CreateSubCategory(SubCategoryEntity subCategoryEntity)
    {
        return await repository.CreateSubCategory(subCategoryEntity);
    }

    public async Task<SubCategoryEntity?> GetSubCategory(long id)
    {
        return await repository.Get(id);
    }

    public async Task<(long, long)> UpdateSubCategory(long id, string name, long parentCategoryId)
    {
        return await repository.Update(id, name, parentCategoryId);
    }

    public async Task<bool> DeleteSubCategory(long id)
    {
        return await repository.Delete(id);
    }

    public async Task<List<SubCategoryEntity>> GetSubCategories()
    {
        return await repository.GetAll();
    }
}