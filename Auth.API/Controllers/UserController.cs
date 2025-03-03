using Auth.Application.Common;
using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Errors;
using Auth.Application.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
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
}