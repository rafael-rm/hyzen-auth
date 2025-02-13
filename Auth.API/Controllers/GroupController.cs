using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Domain.Exceptions.Group;
using Auth.Domain.Exceptions.Role;
using Auth.Domain.Exceptions.User;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GroupController : ControllerBase
{
    private readonly IGroupApplicationService _groupApplicationService;
    
    public GroupController(IGroupApplicationService groupApplicationService)
    {
        _groupApplicationService = groupApplicationService;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(GroupResponse),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync(CreateGroupRequest roleRequest)
    {
        try
        {
            var role = await _groupApplicationService.CreateAsync(roleRequest);
            return Created(string.Empty, role);
        }
        catch (GroupAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
    }
    
    [HttpGet("guid/{guid:guid}")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByGuidAsync(Guid guid)
    {
        try
        {
            var role = await _groupApplicationService.GetByGuidAsync(guid);
            return Ok(role);
        }
        catch (GroupNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpGet("name/{name}")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByEmailAsync(string name)
    {
        try
        {
            var role = await _groupApplicationService.GetByNameAsync(name);
            return Ok(role);
        }
        catch (GroupNotFoundException ex)
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
            await _groupApplicationService.DeleteAsync(name);
            return NoContent();
        }
        catch (GroupNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}