using Gamma.RoboKP.Domain.Abstractions.Repositories;
using Gamma.RoboKP.Domain.Entities;
using Gamma.RoboKP.Infrastructure.Context;
using Gamma.RoboKP.Infrastructure.Models;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Gamma.RoboKP.Infrastructure.Repositories;

public class SubCategoryRepository(
    [FromKeyedServices("RepositoryMapper")] IMapper mapper,
    RoboKpDbContext context) : ISubCategoryRepository
{
    // TODO возврат ID вместо bool
    public async Task<bool?> CreateSubCategory(SubCategoryEntity subCategory)
    {
        var category = await context.Categories.FindAsync(subCategory.ParentCategoryId);
        if (category == null) return false;
        var subCategoryDb = mapper.Map<SubCategoryEntity, SubCategory>(subCategory);
        
        await context.AddAsync(subCategoryDb);
        
        if (await context.SaveChangesAsync() <= 0) return null;
        
        return true;
    }

    public async Task<SubCategoryEntity?> Get(long subCategoryId)
    {
        var subCategory = await context.SubCategories.FindAsync(subCategoryId);
        
        return subCategory != null ? mapper.Map<SubCategory, SubCategoryEntity>(subCategory) : null;
    }

    public async Task<List<SubCategoryEntity>> GetAll()
    {
        var subCategories = await context.SubCategories.ToListAsync();
        return mapper.Map<List<SubCategoryEntity>>(subCategories);
    }

    public async Task<(long, long)> Update(long id, string name, long parentCategoryId)
    {
        var subCategory = await context.SubCategories.FindAsync(id);
        if (subCategory == null) return (0, 0);
        if (subCategory.Name != name) subCategory.Name = name;
        if (subCategory.ParentCategoryId != parentCategoryId)
        {
            var category = await context.Categories.FindAsync(parentCategoryId);
            if (category == null) return (0, 0);
            subCategory.ParentCategoryId = parentCategoryId;
        }
        return await context.SaveChangesAsync() > 0 ? (id, parentCategoryId) : (0, 0);
    }

    public async Task<bool> Delete(long id)
    {
        var subCategory = await context.SubCategories.FindAsync(id);
        if (subCategory == null) return false;
        context.SubCategories.Remove(subCategory);
        return await context.SaveChangesAsync() > 0;
    }
}