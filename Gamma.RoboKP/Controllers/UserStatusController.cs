using Gamma.RoboKP.Domain.Abstractions.Services;
using Gamma.RoboKP.Models.UserStatusModels;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Gamma.RoboKP.Controllers;

[ApiController]
[Route("api/user_status")]
public class UserStatusController(
    [FromKeyedServices("ControllerMapper")] IMapper mapper, IDiscountService discountService
    ) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<string>> CreateStatus([FromQuery] string status, [FromQuery] decimal discountPercent)
    {
        var entity = await discountService.Create(status, (long) discountPercent);
        if (entity == null) return BadRequest("Could not create status");
        return Ok(entity);
    }
    
    [HttpGet]
    public async Task<ActionResult<UserStatusResponseDto>> GetAllStatuses()
    {
        var entities = await discountService.GetAll();
        return Ok(mapper.Map<List<UserStatusResponseDto>>(entities));
    }

    [HttpGet("{status}")]
    public async Task<ActionResult<decimal>> GetDiscountPercentByStatus(string status)
    {
        var entity = await discountService.Get(status);
        if (entity == null) return NotFound();
        return Ok(entity.Percent);
    }

    [HttpPut("{status}")]
    public async Task<ActionResult<string>> UpdateStatusDiscountPercent(string status, [FromQuery] decimal discountPercent)
    {
        var result = await discountService.Update(status, (long) discountPercent);
        if (result == null) return NotFound();
        return Ok(status);
    }

    [HttpDelete("{status}")]
    public async Task<ActionResult<string>> DeleteStatus(string status)
    {
        var result = await discountService.Delete(status);
        if (!result) return NotFound();
        return Ok("Статус успешно удалён");
    }
}