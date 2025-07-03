using System.Security.Claims;
using Gamma.RoboKP.Domain.Abstractions.Auth;
using Gamma.RoboKP.Domain.Abstractions.Services;
using Gamma.RoboKP.Domain.Entities;
using Gamma.RoboKP.Filters.ExceptionsFilters;
using Gamma.RoboKP.Models.Authentication;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using AuthOptions = Gamma.RoboKP.Domain.Options.AuthOptions;

namespace Gamma.RoboKP.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController(IOptions<AuthOptions> authOptions,
    IAuthService authService,
    IRefreshTokenService refreshTokenService, 
    [FromKeyedServices("ControllerMapper")] IMapper mapper,
    ITokenService tokenService,
    IMailService mailService,
    IUserService userService) : ControllerBase
{
    private readonly AuthOptions _authOptions = authOptions.Value;
    
    [HttpPost("register")]
    [AuthExceptions]
    public async Task<ActionResult<UserResponse>> Register([FromBody] UserRegisterDto userRegisterDto)
    {
        var user = mapper.Map<UserRegisterDto, User>(userRegisterDto);
        var result =  await authService.Register(user, userRegisterDto.Password);
        
        var token = tokenService.GenerateAccessToken(result);
        var refreshToken = await tokenService.GenerateRefreshToken(result.Id);
        
        Response.Cookies.Append("access_token", token, new CookieOptions //todo: пока что так, но по идее нужно добавлять токены только после подтверждения почты
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(_authOptions.ExpireMinutes),
        });
        
        Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(_authOptions.RefreshTokenExpireDays),
        });
        
        var newUserResponse = mapper.Map<User, UserResponse>(user);
        newUserResponse.Id = user.Id;
        newUserResponse.Role = result.Role.ToString();
        newUserResponse.Token = token;
        newUserResponse.RefreshToken = refreshToken;
        newUserResponse.UserName = user.Email;
        
        return Ok(newUserResponse);
    }
    
    [HttpPost("login")]
    [AuthExceptions]
    public async Task<ActionResult<UserResponse>> Login([FromBody] UserLoginDto userLoginDto)
    {
        var result = await authService.Login(userLoginDto.Email, userLoginDto.Password);
        
        var token = tokenService.GenerateAccessToken(result);
        var refreshToken = await tokenService.GenerateRefreshToken(result.Id);
        
        Response.Cookies.Append("access_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(_authOptions.ExpireMinutes),
        });
        
        Response.Cookies.Append("refresh_token", refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(_authOptions.RefreshTokenExpireDays),
        });
        
        var newUserResponse = mapper.Map<User, UserResponse>(result);
        
        newUserResponse.Token = token;
        newUserResponse.RefreshToken = refreshToken;
        newUserResponse.UserName = result.Email;
        
        return Ok(newUserResponse);
    }
    
    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        await refreshTokenService.DeleteRefreshToken(refreshToken);
        
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");
        
        return Ok(new{message = "успешный выход из аккаунта"});
    }
    
    [Authorize]
    [HttpPost("logout/all")]
    public async Task<ActionResult> LogoutAll()
    {
        var userIdFromClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdFromClaims == null) return Unauthorized();
        
        var userId = long.Parse(userIdFromClaims);
        
        await refreshTokenService.DeleteAllUserRefreshTokens(userId);
        
        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");
        return Ok();
    }
    
    [HttpPost("refresh")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken)) return Unauthorized();
        
        var result = await authService.RefreshAccessToken(refreshToken);
        if (result is null)
        {
            return NotFound();
        }
        var token = tokenService.GenerateAccessToken(result);
        
        Response.Cookies.Append("access_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(_authOptions.ExpireMinutes),
        });
        return Ok(result);
    }

    [HttpPost("forgotpassword")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        if(!ModelState.IsValid) return BadRequest(ModelState);
        
        var token = await userService.GeneratePasswordResetTokenAsync(forgotPasswordDto.Email!);
        
        if(token is null) return BadRequest("User not found");

        var param = new Dictionary<string, string?>
        {
            { "token", token },
            { "email", forgotPasswordDto.Email }
        };
        var callback = QueryHelpers.AddQueryString(forgotPasswordDto.ClientUri!, param);
        
        var message = new MailData(forgotPasswordDto.Email!, null!, "Reset password token", callback);
        mailService.SendMail(message);
        return Ok();
    }

    [HttpPost("resetpassword")]
    public async Task<ActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
    {
        if(!ModelState.IsValid) return BadRequest();
        
        var result = await userService.ResetPassword(resetPasswordDto.Email!, resetPasswordDto.Token!, resetPasswordDto.Password!);
        
        if(result is null) return NotFound();

        if (result.Succeeded) return Ok();
        
        var errors = result.Errors.Select(e => e.Description);
        return BadRequest(new {Errors = errors});

    }

    [HttpPost("confirm-email")]
    public async Task<ActionResult> ConfirmEmail(ConfirmEmailRequest confirmEmailRequest)
    {
        var result = await mailService.ConfirmMail(confirmEmailRequest.Email, confirmEmailRequest.Code);
        if (result)
        {
            return Ok(new {Message = "Почта успешно подтверждена"});
            
        }
        return BadRequest(new { Message = "Неверный код подтверждения" });
    }
}