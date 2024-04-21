using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using HyzenAuth.Core.DTO.Request.Auth;
using HyzenAuth.Core.DTO.Response.Auth;
using HyzenAuth.Core.Helper;
using HyzenAuth.Core.Infrastructure;
using HyzenAuth.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyzenAuth.Core.Controllers;

[ApiController]
[Route("api/v1/Auth")]
[Authorize]
public class AuthController : ControllerBase
{
    [HttpPost, Route("Login"), AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        await using var context = AuthContext.Get("Auth.Login");
        
        var user = await Models.User.GetAsync(request.Email);

        if (user is null || !PasswordHelper.Verify(request.Password, user.Password))
            return NotFound("User not found or invalid password");
        
        if (!user.IsActive)
            return Forbid("User is not active");

        var token = TokenService.GenerateToken(user, 3);
        var response = new LoginResponse { Token = token };

        return Ok(response);
    }
    
    [HttpPost, Route("Verify"), AllowAnonymous]
    public async Task<IActionResult> Verify()
    {
        await using var context = AuthContext.Get("Auth.Verify");

        var token = TokenHelper.GetToken(HttpContext);
        var subject = TokenService.GetSubjectFromToken(token);

        return Ok(subject);
    }
}