using Gamma.RoboKP.Domain.Entities;

namespace Gamma.RoboKP.Domain.Abstractions.Repositories;

public interface ISubCategoryRepository
{
    Task<bool?> CreateSubCategory(SubCategoryEntity subCategory);
    Task<SubCategoryEntity?> Get(long subCategoryId);
    Task<List<SubCategoryEntity>> GetAll();
    Task<(long, long)> Update(long id, string name, long parentCategoryId);
    Task<bool> Delete(long id);
}