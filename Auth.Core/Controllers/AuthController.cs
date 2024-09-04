using Auth.Core.DTOs.Request.Auth;
using Auth.Core.Infrastructure;
using Auth.Core.Models;
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
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)] 
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        await using var context = AuthContext.Get("Auth.Login");
        
        var user = await Models.User.GetAsync(request.Email);
        
        if (user is null || !HashService.Verify(request.Password, user.Password))
            throw new HException("User not found or invalid password", ExceptionType.NotFound);
        
        if (!user.IsActive)
            throw new HException("User is not active", ExceptionType.InvalidOperation);

        var token = TokenService.GenerateToken(user, out var issuedAt, 3);
        user.RegisterLoginEvent(issuedAt);

        var subject = await TokenService.GetSubjectFromToken(token);
        var response = new LoginResponse
        {
            Token = token,
            Subject = subject
        };
        
        await context.SaveChangesAsync();
        return Ok(response);
    }
    
    [HttpPost, Route("Verify")]
    [ProducesResponseType(typeof(AuthSubject), StatusCodes.Status200OK)] 
    public async Task<IActionResult> Verify()
    {
        await using var context = AuthContext.Get("Auth.Verify");
        var subject = await TokenService.GetSubjectFromToken(HyzenAuth.GetToken());
        return Ok(subject);
    }
    
    [HttpPost, Route("SendRecoveryEmail")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendRecoveryEmail([FromForm] string email)
    {
        await using var context = AuthContext.Get("User.SendRecoveryEmail");

        var user = await Models.User.GetAsync(email);

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);

        var verificationCode = await VerificationCode.CreateAsync(user.Id, DateTime.Now.AddMinutes(15));
        
        var dynamicTemplateData = new
        {
            displayName = user.Name,
            recoveryCode = VerificationCode.FormatVerificationCode(verificationCode.Code),
            recoveryUrl = $"https://hyzen.com.br/recovery?email={Uri.EscapeDataString(user.Email)}&code={verificationCode.Code}"
        };
        
        var response = await HyzenMail.SendTemplateMail("noreply@hyzen.com.br", user.Email, "d-22d1b8b0c8df4ce9ae516cf171a1fa58", dynamicTemplateData);
        
        if (!response)
            throw new HException("Failed to send recovery email", ExceptionType.InvalidOperation);
        
        user.RegisterRecoveryPasswordEvent();
        await context.SaveChangesAsync();
        
        return Ok(true);
    }
}