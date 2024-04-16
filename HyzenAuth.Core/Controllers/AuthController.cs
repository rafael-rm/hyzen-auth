using HyzenAuth.Core.DTO.Request.Auth;
using HyzenAuth.Core.DTO.Response.Auth;
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

        if (string.IsNullOrEmpty(request.Email))
            return BadRequest("Email not provided");
        
        var user = await Models.User.GetAsync(request.Email);

        if (user is null)
            return NotFound("User not found");
        
        if (!PasswordHelper.Verify(request.Password, user.Password))
            return NotFound("User not found");

        var token = TokenService.GenerateToken(user, 3);
        var response = new LoginResponse { Token = token };

        return Ok(response);
    }
}