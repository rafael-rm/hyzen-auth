using Auth.Core.DTOs.Enum;
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

        if (user is null)
            throw new HException("User not found or invalid password", ExceptionType.NotFound);
        
        var requestCount = await Event.CountAsync(user.Id, EventType.LoginFailed, DateTime.Now.AddHours(-1), DateTime.Now);

        if (requestCount >= 10)
        {
            await Event.Register(user.Id, EventType.LoginAttemptAfterLockout, "Login attempt after lockout");
            throw new HException("You have reached the limit of login attempts. Please try again later.", ExceptionType.InvalidOperation);
        }

        if (!HashService.Verify(request.Password, user.Password))
        {
            await Event.Register(user.Id, EventType.LoginFailed, "Invalid password");
            throw new HException("User not found or invalid password", ExceptionType.NotFound);
        }

        if (!user.IsActive)
        {
            await Event.Register(user.Id, EventType.LoginFailed, "User is not active");
            throw new HException("User is not active", ExceptionType.InvalidOperation);
        }

        var token = TokenService.GenerateToken(user, out var issuedAt, 3);
        await user.RegisterLoginEvent(issuedAt);

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
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> SendRecoveryEmail([FromForm] string email)
    {
        await using var context = AuthContext.Get("User.SendRecoveryEmail");

        var user = await Models.User.GetAsync(email);

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);
        
        var requestCount = await Event.CountAsync(user.Id, EventType.PasswordRecoveryRequested, DateTime.Now.AddHours(-1), DateTime.Now);
        
        if (requestCount >= 5)
        {
            await Event.Register(user.Id, EventType.PasswordRecoveryRequestLimitReached, "Password recovery request limit reached");
            throw new HException("You have reached the limit of password recovery requests. Please try again later.", ExceptionType.InvalidOperation);
        }

        var verificationCode = await VerificationCode.CreateAsync(user.Id, DateTime.Now.AddMinutes(15), VerificationCodeType.PasswordRecovery);
        
        var dynamicTemplateData = new
        {
            displayName = user.Name,
            recoveryCode = VerificationCode.FormatVerificationCode(verificationCode.Code),
            recoveryUrl = $"https://hyzen.com.br/recovery?email={Uri.EscapeDataString(user.Email)}&code={verificationCode.Code}"
        };
        
        var response = await HyzenMail.SendTemplateMail("noreply@hyzen.com.br", user.Email, "d-22d1b8b0c8df4ce9ae516cf171a1fa58", dynamicTemplateData);
        
        if (!response)
            throw new HException("Failed to send recovery email", ExceptionType.InvalidOperation);
        
        await Event.Register(user.Id, EventType.PasswordRecoveryRequested, "Recovery email sent");
        await context.SaveChangesAsync();
        
        return Ok();
    }
    
    [HttpPost, Route("RecoverPassword")]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task<IActionResult> RecoverPassword([FromBody] RecoverPasswordRequest request)
    {
        await using var context = AuthContext.Get("User.RecoverPassword");

        var user = await Models.User.GetAsync(request.Email);

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);
        
        var requestCount = await Event.CountAsync(user.Id, EventType.PasswordRecoveryFailed, DateTime.Now.AddHours(-1), DateTime.Now);
        if (requestCount >= 5)
        {
            await Event.Register(user.Id, EventType.PasswordRecoveryAttemptAfterLockout, "Password recovery limit reached");
            throw new HException("You have reached the limit of password recovery attempts. Please try again later.", ExceptionType.InvalidOperation);
        }

        var verificationCode = await VerificationCode.GetAsync(user.Id, request.VerificationCode, VerificationCodeType.PasswordRecovery);

        if (verificationCode is null)
        {
            await Event.Register(user.Id, EventType.PasswordRecoveryFailed, $"Invalid verification code {request.VerificationCode}");
            throw new HException("Invalid verification code", ExceptionType.InvalidOperation);
        }

        verificationCode.Ensure(user, VerificationCodeType.PasswordRecovery);
        verificationCode.UseAsync();
        
        await user.ChangePassword(request.NewPassword);
        await Event.Register(user.Id, EventType.PasswordRecovered, $"Password recovered using code {request.VerificationCode}");
        await context.SaveChangesAsync();
        
        return Ok();
    }
}