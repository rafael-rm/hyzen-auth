using Auth.Core.DTO.Request.Auth;
using Auth.Core.DTO.Response.Auth;
using Auth.Core.Infrastructure;
using Auth.Core.Services;
using Hyzen.SDK.Authentication;
using Hyzen.SDK.Exception;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Core.Controllers;

[ApiController]
[Route("api/v1/Auth")]
public class AuthController : ControllerBase
{
    [HttpPost, Route("Login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)] 
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        await using var context = AuthContext.Get("Auth.Login");
        
        var user = await Models.User.GetAsync(request.Email);
        
        if (user is null || !PasswordHelper.Verify(request.Password, user.Password))
            throw new HException("User not found or invalid password", ExceptionType.NotFound);
        
        if (!user.IsActive)
            throw new HException("User is not active", ExceptionType.InvalidOperation);

        var token = TokenService.GenerateToken(user, 3);
        user.RegisterLoginEvent();

        await context.SaveChangesAsync();

        var response = new LoginResponse(token);
        return Ok(response);
    }
    
    [HttpPost, Route("Verify")]
    [ProducesResponseType(typeof(VerifyResponse), StatusCodes.Status200OK)] 
    public async Task<IActionResult> Verify()
    {
        await using var context = AuthContext.Get("Auth.Verify");
        var subject = TokenService.GetSubjectFromToken(HyzenAuth.GetToken());
        return Ok(subject);
    }
}