using HyzenAuth.Core.DTO.Request;
using HyzenAuth.Core.DTO.Response;
using HyzenAuth.Core.Infrastructure;
using HyzenAuth.Core.Models;
using HyzenAuth.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HyzenAuth.Core.Controllers;

[ApiController]
[Route("api/v1/Auth")]
public class AuthController : ControllerBase
{
    [HttpPost, Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        await using var context = AuthContext.Get("Auth.Login");
        
        var user = await Models.User.GetAsync(request);

        if (user is null)
            throw new Exception("User not found");

        var token = TokenService.GenerateToken(user);
        
        var response = LoginResponse.FromUser(user, token);
        
        await context.SaveChangesAsync();

        return Ok(response);
    }
}