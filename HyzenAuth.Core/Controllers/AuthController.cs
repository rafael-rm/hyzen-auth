﻿using Hyzen.SDK.Authentication;
using Hyzen.SDK.Exception;
using HyzenAuth.Core.DTO.Request.Auth;
using HyzenAuth.Core.DTO.Response.Auth;
using HyzenAuth.Core.Infrastructure;
using HyzenAuth.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyzenAuth.Core.Controllers;

[ApiController]
[Route("api/v1/Auth")]
public class AuthController : ControllerBase
{
    [HttpPost, Route("Login"), AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        await using var context = AuthContext.Get("Auth.Login");
        
        var user = await Models.User.GetAsync(request.Email);
        
        if (user is null || !PasswordHelper.Verify(request.Password, user.Password))
            throw new HException("User not found or invalid password", ExceptionType.NotFound);
        
        if (!user.IsActive)
            throw new HException("User is not active", ExceptionType.InvalidOperation);

        var token = TokenService.GenerateToken(user, 3);
        var response = new LoginResponse { Token = token };

        return Ok(response);
    }
    
    [HttpPost, Route("Verify"), AllowAnonymous]
    public async Task<IActionResult> Verify()
    {
        await using var context = AuthContext.Get("Auth.Verify");
        var subject = TokenService.GetSubjectFromToken(Auth.GetToken());
        return Ok(subject);
    }
}