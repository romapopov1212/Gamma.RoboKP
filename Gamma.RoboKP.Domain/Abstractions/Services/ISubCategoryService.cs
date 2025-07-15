using Gamma.RoboKP.Domain.Entities;

namespace Gamma.RoboKP.Domain.Abstractions.Services;

public interface ISubCategoryService
{
    Task<bool?> CreateSubCategory(SubCategoryEntity subCategoryEntity);
    Task<SubCategoryEntity?> GetSubCategory(long id);
    Task<List<SubCategoryEntity>> GetSubCategories();
    Task<(long, long)> UpdateSubCategory(long id, string name, long parentCategoryId);
    Task<bool> DeleteSubCategory(long id);
}