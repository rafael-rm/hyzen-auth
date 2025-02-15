using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Domain.Exceptions.Group;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GroupController : ControllerBase
{
    private readonly IGroupService _groupService;
    
    public GroupController(IGroupService groupService)
    {
        _groupService = groupService;
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(GroupResponse),StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateAsync(CreateGroupRequest request)
    {
        try
        {
            var response = await _groupService.CreateAsync(request);
            return Created(string.Empty, response);
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
            var response = await _groupService.GetByGuidAsync(guid);
            return Ok(response);
        }
        catch (GroupNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpGet("key/{key}")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetByKeyAsync(string key)
    {
        try
        {
            var response = await _groupService.GetByKeyAsync(key);
            return Ok(response);
        }
        catch (GroupNotFoundException ex)
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
            await _groupService.DeleteAsync(key);
            return NoContent();
        }
        catch (GroupNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
    
    [HttpPut("key/{key}")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateByKeyAsync(string key, UpdateGroupRequest request)
    {
        try
        {
            var response = await _groupService.UpdateAsync(key, request);
            return Ok(response);
        }
        catch (GroupNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}