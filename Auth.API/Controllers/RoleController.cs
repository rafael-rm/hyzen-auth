using Auth.Application.Common;
using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Errors;
using Auth.Application.Interfaces.ApplicationServices;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RoleController(IRoleService roleService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Result<RoleResponse>),StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync(CreateRoleRequest roleRequest)
    { 
        var result = await roleService.CreateAsync(roleRequest);
        
        if (result.IsSuccess)
            return Created(string.Empty, result);
        
        if (result.Error == RoleError.RoleAlreadyExists)
            return Conflict(result);
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    [HttpGet("guid/{guid:guid}")]
    [ProducesResponseType(typeof(Result<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByGuidAsync(Guid guid)
    {

        var result = await roleService.GetByGuidAsync(guid);

        if (result.IsSuccess)
            return Ok(result);

        if (result.Error == RoleError.RoleNotFound)
            return NotFound(result);
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
    
    [HttpGet("key/{key}")]
    [ProducesResponseType(typeof(Result<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByKeyAsync(string key)
    {
        var result = await roleService.GetByKeyAsync(key);
        
        if (result.IsSuccess)
            return Ok(result);

        if (result.Error == RoleError.RoleNotFound)
            return NotFound(result);
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
    
    [HttpDelete("key/{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteByKeyAsync(string key)
    {
        var result = await roleService.DeleteAsync(key);
        
        if (result.IsSuccess)
            return NoContent();
        
        if (result.Error == RoleError.RoleNotFound)
            return NotFound(result);
        
        return StatusCode(StatusCodes.Status500InternalServerError);
    }
}