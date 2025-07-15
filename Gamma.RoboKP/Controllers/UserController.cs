using System.Security.Claims;
using Gamma.RoboKP.Domain.Abstractions.Services;
using Gamma.RoboKP.Domain.Entities;
using Gamma.RoboKP.Domain.Enums;
using Gamma.RoboKP.Domain.ValueObject;
//using Gamma.RoboKP.Domain.Models;
using Gamma.RoboKP.Filters.ExceptionsFilters;
using Gamma.RoboKP.Models.CompanyDto;
using Gamma.RoboKP.Models.User;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gamma.RoboKP.Controllers;
[ApiController]
[Route("api/users")]
public class UserController(IUserService userService, [FromKeyedServices("ControllerMapper")] IMapper mapper) : ControllerBase
{
    
    [HttpGet("{id}/role")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    [AuthExceptions]
    public async Task<ActionResult<string>> GetUserRole([FromRoute] long id)
    {
        var result = await userService.GetUserRole(id);
        
        if (result is null)
        {
            return NotFound($"Пользователь с id {id} не найден");
        }
        return Ok(result);
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserToGet>> GetUserByEmail([FromRoute] string email)
    {
        var user = await userService.GetUserByEmail(email);
        
        if (user is null) return NotFound();
        
        var response = mapper.Map<User, UserToGet>(user);
        
        return Ok(response);
    }

    [Authorize(Roles =nameof(UserRole.Admin))]
    [HttpGet("{id}")]
    [AuthExceptions]
    public async Task<ActionResult<UserToGet>> GetUser([FromRoute] long id)
    {
        var user = await userService.GetUserById(id);
        if (user is null) return NotFound();
        
        var response = mapper.Map<User, UserToGet>(user);
        
        return Ok(response);
    }
    
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet]
    public async Task<ActionResult<List<UserToGet>>> GetUsers()
    {
        var users = await userService.GetAllUsers();
        
        var response = mapper.Map<List<User>, List<UserToGet>>(users);
        
        return Ok(response);
    }
    
    [Authorize(Roles = nameof(UserRole.Admin))] //TODO: admingamma как сonst
    [HttpPatch("{id}/role")]
    [AuthExceptions]
    public async Task<ActionResult<bool>> SetRole([FromRoute] long id, [FromHeader] string role)
    {
        var result = await userService.SetUserRole(id, role);
        if (result == null) return BadRequest("Ошибка базы данных");
        if (!result.Value.Item1) return NotFound($"Не существует пользователя с id: {id}");
        if (!result.Value.Item2) return NotFound($"Не существует роли: {role}");
        return Ok();
    }
    
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpGet("{id}/status")]
    [AuthExceptions]
    public async Task<ActionResult<string>> GetStatus([FromRoute] long id)
    {
        var result = await userService.GetUserStatus(id);
        return Ok(result);
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpPatch("{id}/setStatus")]
    [AuthExceptions]
    public async Task<ActionResult> SetStatus([FromRoute] long id, [FromHeader] string status)
    {
        var result = await userService.SetStatus(id, status);
        if (result == null) return BadRequest("Ошибка базы данных");
        if (!result.Value.Item1) return NotFound($"Не существует пользователя с id: {id}");
        if (!result.Value.Item2) return NotFound($"Не существует статуса: {status}");
        return Ok();
    }
    
    [Authorize]
    [HttpPatch("me")]
    public async Task<ActionResult> Update([FromBody] UserToUpdate userToUpdate)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId is null) return Unauthorized();
        
        var longUserId = long.Parse(userId);
        
        var response = await userService.UpdateUser(longUserId, userToUpdate.FirstName, userToUpdate.SurName, userToUpdate.LastName, userToUpdate.Email);
        
        if (response) return Ok(); 
        return BadRequest("Invalid token");
    }

    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{id}")]
    public async Task<ActionResult<bool>> Delete([FromRoute] long id)
    {
        var response = await userService.DeleteUser(id);
        if (response) return Ok();
        
        return NotFound();
    }

    [Authorize]
    [HttpPatch("user/profile/company")]
    public async Task<ActionResult<bool>> UpdateProfile([FromBody] CompanyToAdd companyToAdd)
    {
        var userIdFromClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdFromClaims is null) return Unauthorized();
        
        long userId = long.Parse(userIdFromClaims);
        var company = mapper.Map<CompanyToAdd, Company>(companyToAdd);
        var response = await userService.SetCompanyInfo(company, userId);
        
        if (response) return Ok();
        return BadRequest(response);
    }
}
