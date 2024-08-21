using Auth.Core.DTOs.Request.Auth;
using Auth.Core.DTOs.Response.Auth;
using Auth.Core.Infrastructure;
using Auth.Core.Services;
using Hyzen.SDK.Authentication;
using Hyzen.SDK.Authentication.DTO;
using Hyzen.SDK.Email;
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
        
        if (user is null || !HashService.Verify(request.Password, user.Password))
            throw new HException("User not found or invalid password", ExceptionType.NotFound);
        
        if (!user.IsActive)
            throw new HException("User is not active", ExceptionType.InvalidOperation);

        var token = TokenService.GenerateToken(user, TokenType.User, 3, out var issuedAt);
        user.RegisterLoginEvent(issuedAt);

        await context.SaveChangesAsync();
        
        var subject = await TokenService.GetSubjectFromToken(token);

        var response = new LoginResponse(subject, token);
        return Ok(response);
    }
    
    [HttpPost, Route("Verify")]
    [ProducesResponseType(typeof(VerifyResponse), StatusCodes.Status200OK)] 
    public async Task<IActionResult> Verify()
    {
        await using var context = AuthContext.Get("Auth.Verify");
        var subject = await TokenService.GetSubjectFromToken(HyzenAuth.GetToken());
        return Ok(subject);
    }
    
    [HttpPost, Route("RecoveryPassword")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> RecoveryPassword([FromForm] string email)
    {
        await using var context = AuthContext.Get("Auth.RecoveryPassword");

        var user = await Models.User.GetAsync(email);

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);
        
        var recoveryToken = TokenService.GenerateToken(user, TokenType.Recovery, 1, out _);
        
        var dynamicTemplateData = new
        {
            displayName = user.Name,
            recoveryUrl = $"https://hyzen.com.br/auth/recovery/{recoveryToken}"
        };
        
        var response = await HyzenMail.SendTemplateMail("noreply@hyzen.com.br", user.Email, "d-22d1b8b0c8df4ce9ae516cf171a1fa58", dynamicTemplateData);
        
        if (!response)
            throw new HException("Failed to send recovery email", ExceptionType.InvalidOperation);
        
        user.RegisterRecoveryPasswordEvent();
        
        await context.SaveChangesAsync();

        return Ok(true);
    }
}