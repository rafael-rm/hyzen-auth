﻿using Auth.Application.Common;
using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Errors;
using Auth.Application.Interfaces.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(typeof(Result<UserResponse>),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync(CreateUserRequest userRequest)
    {
        var result = await userService.CreateAsync(userRequest);
        
        if (result.IsSuccess)
            return Created(string.Empty, result);
        
        if (result.Error == UserError.UserAlreadyExists)
            return Conflict(result);
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
    
    [Authorize(Roles = "user")]
    [HttpGet("guid/{guid:guid}")]
    [ProducesResponseType(typeof(Result<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByGuidAsync(Guid guid)
    {
        
        var result = await userService.GetByGuidAsync(guid);
            
        if (result.IsSuccess)
            return Ok(result);
        
        if (result.Error == UserError.UserNotFound)
            return NotFound(result);
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
    
    [Authorize(Roles = "admin")]
    [HttpGet("email/{email}")]
    [ProducesResponseType(typeof(Result<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByEmailAsync(string email)
    {
        var result = await userService.GetByEmailAsync(email);
        
        if (result.IsSuccess)
            return Ok(result);
    
        if (result.Error == UserError.UserNotFound)
            return NotFound(result);
    
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
    
    [Authorize(Roles = "admin")]
    [HttpDelete("guid/{guid:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteByGuidAsync(Guid guid)
    {
        var result = await userService.DeleteAsync(guid);
        
        if (result.IsSuccess) 
            return NoContent();
            
        if (result.Error == UserError.UserNotFound)
            return NotFound(result);
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
    
    [Authorize(Roles = "admin")]
    [HttpPut("guid/{guid:guid}/roles")]
    [ProducesResponseType(typeof(Result<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateRolesAsync(Guid guid, UpdateUserRolesRequest request)
    {
        var result = await userService.UpdateRolesAsync(guid, request.Roles);
        
        if (result.IsSuccess)
            return Ok(result);
        
        if (result.Error == UserError.UserNotFound)
            return NotFound(result);
        
        if (result.Error == RoleError.RoleNotFound)
            return NotFound(result);
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}