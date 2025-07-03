using System.Security.Claims;
using Gamma.RoboKP.Domain.Abstractions.Services;
using Gamma.RoboKP.Models.Cart;
using Gamma.RoboKP.Models.Product;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Gamma.RoboKP.Controllers;

[ApiController]
[Route("api/carts")]
public class CartController(
    ICartService cartService,
    [FromKeyedServices("ControllerMapper")] IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CartResponseDto>>> GetAllCarts()
    {
        var result = await cartService.GetAll();
        var cart = mapper.Map<List<CartResponseDto>>(result);
        return Ok(cart);
    }

    [HttpGet("products")]
    public async Task<ActionResult<List<ProductResponseDto>>> GetProducts()
    {
        var userId = "1234";
        if (userId == null) return Unauthorized();
        var result = await cartService.GetProducts(userId);
        if (result == null) return NotFound();
        return Ok(mapper.Map<List<ProductResponseDto>>(result));
    }

    [HttpPost("{productId}")]
    public async Task<ActionResult> AddProduct(long productId)
    {
        var userId = "1234";
        if (userId == null) return Unauthorized();
        var result = await cartService.AddProduct(userId, productId);
        return !result.IsNullOrEmpty() ? Ok() : NotFound();
    }
    
    [HttpDelete("{productId}")]
    public async Task<ActionResult> RemoveProduct(long productId)
    {
        var userId = "1234";
        if (userId == null) return Unauthorized();
        var result = await cartService.RemoveProduct(userId, productId);
        return !result.IsNullOrEmpty() ? Ok() : NotFound();
    }

    [HttpPatch("clean")]
    public async Task<ActionResult> CleanCart()
    {
        var userId = "1234";
        if (userId == null) return Unauthorized();
        var result = await cartService.Flush(userId);
        return result ? Ok() : NotFound();
    }

    [HttpGet("cost")]
    public async Task<ActionResult<long>> GetCost()
    {
        var userStatus = User.FindFirst(ClaimTypes.UserData)?.Value;
        var userId = "1234";
        if (userStatus == null || userId == null) return Unauthorized();
        var result = await cartService.GetTotalCost(userId);
        return result != null ? Ok(result) : NotFound();
    }
}