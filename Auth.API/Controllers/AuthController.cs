using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Exceptions;
using Auth.Application.Interfaces;
using Auth.Domain.Exceptions.User;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        try
        {
            var user = await _authService.LoginAsync(request.Email, request.Password);
            return Ok(user);
        }
        catch (AuthenticationFailedException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
    
    [HttpGet("verify/{token}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Verify(string token)
    {
        try
        {
            await _authService.VerifyAsync(token);
            return Ok();
        }
        catch (InvalidTokenException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}