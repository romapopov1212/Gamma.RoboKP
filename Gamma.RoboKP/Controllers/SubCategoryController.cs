using Gamma.RoboKP.Domain.Abstractions.Services;
using Gamma.RoboKP.Domain.Entities;
using Gamma.RoboKP.Models.SubCategory;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Gamma.RoboKP.Controllers;

[ApiController]
[Route("api/subcategories")]
public class SubCategoryController(
    [FromKeyedServices("RepositoryMapper")] IMapper mapper,
    ISubCategoryService subCategoryService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<(long, long)>> CreateSubCategory(string name, long parentCategoryId)
    {
        var subCategoryEntity = SubCategoryEntity.Create(name, parentCategoryId);
        
        var response = await subCategoryService.CreateSubCategory(subCategoryEntity);

        if (response == null) return BadRequest("Ошибка БД");
        if (!response.Value)
            return NotFound($"Не существует родительской категории с ID: {parentCategoryId}");
        
        return Ok(response.Value);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<SubCategoryResponseDto>> GetSubCategory(long id)
    {
        var subCategoryEntity = await subCategoryService.GetSubCategory(id);
        return subCategoryEntity != null ? Ok(mapper.Map<SubCategoryResponseDto>(subCategoryEntity)) : NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<List<(long, long)>>> GetSubCategories()
    {
        var subCategories = await subCategoryService.GetSubCategories();
        return Ok(mapper.Map<List<SubCategoryResponseDto>>(subCategories));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<(long, long)>> UpdateSubCategory(long id, string name, long parentCategoryId)
    {
        var result = await subCategoryService.UpdateSubCategory(id, name, parentCategoryId);
        return result.Item1 != 0 &&  result.Item2 != 0 ? Ok((result.Item1, result.Item2)) : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteSubCategory(long id)
    {
        var result = await subCategoryService.DeleteSubCategory(id);
        return result ? Ok() : NotFound();
    }
}