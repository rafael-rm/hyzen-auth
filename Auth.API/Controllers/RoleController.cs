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
    
    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByEmailAsync(string name)
    {
        try
        {
            var role = await _roleService.GetByNameAsync(name);
            return Ok(role);
        }
        catch (RoleNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpDelete("name/{name}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteByNameAsync(string name)
    {
        try
        {
            await _roleService.DeleteAsync(name);
            return NoContent();
        }
        catch (RoleNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}