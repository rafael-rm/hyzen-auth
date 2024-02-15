using HyzenAuth.Core.DTO.Request;
using HyzenAuth.Core.DTO.Response;
using HyzenAuth.Core.Infrastructure;
using HyzenAuth.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace HyzenAuth.Core.Controllers;

[ApiController]
[Route("api/v1/Auth")]
public class AuthController : ControllerBase
{
    [HttpPost, Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        await using var context = AuthContext.Get("Auth.Login");
        
        var user = (await Models.User.SearchAsync(email: request.Email, isActive: true)).FirstOrDefault();

        if (user is null || !PasswordHelper.Verify(request.Password, user.Password))
            return NotFound("User not found");

        var token = TokenService.GenerateToken(user, 12);
        var response = new LoginResponse { Token = token };
        
        await context.SaveChangesAsync();

        return Ok(response);
    }
}