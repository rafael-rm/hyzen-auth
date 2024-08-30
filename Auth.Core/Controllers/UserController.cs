﻿using Auth.Core.DTOs.Request.User;
using Auth.Core.DTOs.Response.User;
using Auth.Core.Infrastructure;
using Auth.Core.Models;
using Hyzen.SDK.Authentication;
using Hyzen.SDK.Email;
using Hyzen.SDK.Exception;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Core.Controllers;

[ApiController]
[Route("api/v1/User")]
public class UserController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] Guid id)
    {
        await HyzenAuth.EnsureRole("hyzen_auth:user:get");
        await using var context = AuthContext.Get("User.Get");
        
        var user = await Models.User.GetAsync(id);

        if (user is null)
            return NotFound("User not found");
        
        var response = UserResponse.FromUser(user);

        return Ok(response);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        await using var context = AuthContext.Get("User.Create");
        
        var user = await Models.User.GetAsync(request.Email);

        if (user is not null)
            throw new HException("There is already a user with this email", ExceptionType.InvalidOperation);

        user = await Models.User.CreateAsync(request);
        
        var response = UserResponse.FromUser(user);
        await context.SaveChangesAsync();

        return Ok(response);
    }
    
    [HttpDelete]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromForm] Guid id)
    {
        await HyzenAuth.EnsureRole("hyzen_auth:user:delete");
        await using var context = AuthContext.Get("User.Delete");
        
        var user = await Models.User.GetAsync(id);

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);
        
        user.Delete();
        await context.SaveChangesAsync();

        return Ok(true);
    }
    
    [HttpPut]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromForm] Guid userGuid, [FromBody] UpdateUserRequest request)
    {
        await HyzenAuth.EnsureAdmin(); // TODO: Ensure user is admin or user is updating itself
        await using var context = AuthContext.Get("User.Update");
        
        var user = await Models.User.GetAsync(userGuid);;

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);
        
        user.Update(request);
        
        var response = UserResponse.FromUser(user);
        await context.SaveChangesAsync();

        return Ok(response);
    }
    
    [HttpPost, Route("HasRole")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> HasRole([FromForm] Guid userGuid, [FromForm] string roleName)
    {
        await HyzenAuth.EnsureRole("hyzen_auth:user:has_role");
        await using var context = AuthContext.Get("User.HasRole");

        var user = await Models.User.GetAsync(userGuid);

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);

        var hasRole = await user.HasRole(roleName);

        return Ok(hasRole);
    }
    
    [HttpPost, Route("HasGroup")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> HasGroup([FromForm] Guid userGuid, [FromForm] string groupName)
    {
        await HyzenAuth.EnsureRole("hyzen_auth:user:has_group");
        await using var context = AuthContext.Get("User.HasGroup");

        var user = await Models.User.GetAsync(userGuid);

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);

        var hasGroup = await user.HasGroup(groupName);

        return Ok(hasGroup);
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
        
        // TODO: Ajustar template para exibir o código de recuperação no email
        var dynamicTemplateData = new
        {
            displayName = user.Name,
            recoveryUrl = $"https://hyzen.com.br/{verificationCode.Code}"
        };
        
        var response = await HyzenMail.SendTemplateMail("noreply@hyzen.com.br", user.Email, "d-22d1b8b0c8df4ce9ae516cf171a1fa58", dynamicTemplateData);
        
        if (!response)
            throw new HException("Failed to send recovery email", ExceptionType.InvalidOperation);
        
        user.RegisterRecoveryPasswordEvent();
        
        await context.SaveChangesAsync();

        return Ok(true);
    }
}