﻿using Auth.Core.DTO.Request.User;
using Auth.Core.DTO.Response.User;
using Auth.Core.Infrastructure;
using Auth.Core.Models;
using Hyzen.SDK.Authentication;
using Hyzen.SDK.Exception;
using Microsoft.AspNetCore.Authorization;
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
        await HyzenAuth.EnsureRole("hyzen_auth:user:update");
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
        await using var context = AuthContext.Get("Role.HasRole");

        var user = await Models.User.GetAsync(userGuid);

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);

        var hasRole = await user.HasRole(roleName);

        return Ok(hasRole);
    }
}