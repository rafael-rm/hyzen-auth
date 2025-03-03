using Auth.Application.Common;
using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Errors;
using Auth.Application.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(Result<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await authService.LoginAsync(request.Email, request.Password);
        
        if (result.IsSuccess)
            return Ok(result);
        
        if (result.Error == AuthError.InvalidCredentials)
            return Unauthorized(result);
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
    
    [HttpGet("verify/{token}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Verify(string token)
    {
        var result = await authService.VerifyAsync(token);
        
        if (result.IsSuccess)
            return NoContent();
        
        if (result.Error == AuthError.InvalidToken)
            return Unauthorized(result);
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}