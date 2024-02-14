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

        request.Password = PasswordHelper.HashPassword(request.Password);
        
        var user = (await Models.User.SearchAsync(email: request.Email, password: request.Password)).FirstOrDefault();

        if (user is null)
            return NotFound("User not found");

        var token = TokenService.GenerateToken(user);
        var response = LoginResponse.FromUser(user, token);
        
        await context.SaveChangesAsync();

        return Ok(response);
    }
}