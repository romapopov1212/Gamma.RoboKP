using System.Security.Claims;
using Gamma.RoboKP.Domain.Abstractions.Services;
using Gamma.RoboKP.Models.Cart;
using Gamma.RoboKP.Models.Product;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Gamma.RoboKP.Controllers;

[Authorize]
[ApiController]
[Route("api/carts")]
public class CartController(
    ICartService cartService,
    IProductService productService,
    [FromKeyedServices("ControllerMapper")] IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CartResponseDto>>> GetAllCarts()
    {
        
        var cartEntities = await cartService.GetAll();
    
        var result = new List<CartResponseDto>();
    
        foreach (var cartEntity in cartEntities)
        {
            var productsId = cartEntity.Products?.Select(p => p.Id).ToList() ?? new List<long>();
        
            result.Add(new CartResponseDto(
                UserId: cartEntity.UserId,
                ProductsId: productsId
            ));
        }
    
        return Ok(result);
       
    }

    [HttpGet("products")]
    public async Task<ActionResult<List<ProductResponseDto>>> GetProducts()
    {
        var userIdFromClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdFromClaims == null) return Unauthorized();
        
        var userId = int.Parse(userIdFromClaims);
        
        var result = await cartService.GetProducts(userId);
        if (result == null) return NotFound();
        return Ok(mapper.Map<List<ProductResponseDto>>(result));
    }

    [HttpPost("{productId}")]
    public async Task<ActionResult<long>> AddProduct(long productId)
    {
        var userIdFromClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdFromClaims == null) return Unauthorized();
        
        var userId = int.Parse(userIdFromClaims);
        
        var result = await cartService.AddProduct(userId, productId);
        
        return result == null ? NotFound() : Ok(result);
    }
    
    [HttpDelete("{productId}")]
    public async Task<ActionResult> RemoveProduct(long productId)
    {
        var userIdFromClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdFromClaims == null) return Unauthorized();
        
        var userId = int.Parse(userIdFromClaims);
        
        var result = await cartService.RemoveProduct(userId, productId);
        
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPatch("clean")]
    public async Task<ActionResult> CleanCart()
    {
        var userIdFromClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdFromClaims == null) return Unauthorized();
        
        var userId = int.Parse(userIdFromClaims);
        
        var result = await cartService.Flush(userId);
        
        return result ? Ok() : NotFound();
    }

    [HttpGet("cost")]
    public async Task<ActionResult<long>> GetCost()
    {
        var userStatus = User.FindFirst(ClaimTypes.UserData)?.Value;
        if (userStatus == null) return Unauthorized();
        
        var userIdFromClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdFromClaims == null) return Unauthorized();
        
        var userId = int.Parse(userIdFromClaims);
        
        // TODO убрать исключение при запросе
        var cost = await cartService.GetTotalCost(userId);
        if (cost == null) return NotFound();
        
        var discount = await productService.GetDiscount(userStatus);
        if (discount != null) cost = (decimal)((double)cost * discount.GetMultiplier());

        return (long) cost;
    }
}