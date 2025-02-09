using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleApplicationService _roleApplicationService;
    
    public RoleController(IRoleApplicationService roleApplicationService)
    {
        _roleApplicationService = roleApplicationService;
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
            var role = await _roleApplicationService.CreateAsync(roleRequest);
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
            var role = await _roleApplicationService.GetByGuidAsync(guid);
            return Ok(role);
        }
        catch (UserNotFoundException ex)
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
            var role = await _roleApplicationService.GetByNameAsync(name);
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
            await _roleApplicationService.DeleteAsync(name);
            return NoContent();
        }
        catch (RoleNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}