using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Domain.Exceptions.Role;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;
    
    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(RoleResponse),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync(CreateRoleRequest roleRequest)
    {
        try
        {
            var role = await _roleService.CreateAsync(roleRequest);
            return Created(string.Empty, role);
        }
        catch (RoleAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    [HttpGet("guid/{guid:guid}")]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByGuidAsync(Guid guid)
    {
        try
        {
            var role = await _roleService.GetByGuidAsync(guid);
            return Ok(role);
        }
        catch (RoleNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpGet("key/{key}")]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByKeyAsync(string key)
    {
        try
        {
            var role = await _roleService.GetByKeyAsync(key);
            return Ok(role);
        }
        catch (RoleNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpDelete("key/{key}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteByKeyAsync(string key)
    {
        try
        {
            await _roleService.DeleteAsync(key);
            return NoContent();
        }
        catch (RoleNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}