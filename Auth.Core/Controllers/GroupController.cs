using System.Net;
using Auth.Core.DTO.Request.Group;
using Auth.Core.DTO.Response.Group;
using Auth.Core.Infrastructure;
using Auth.Core.Models;
using Hyzen.SDK.Exception;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Core.Controllers;

[ApiController]
[Route("api/v1/Group")]
public class GroupController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GroupResponseWithRoles), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] string name)
    {
        await using var context = AuthContext.Get("Group.Get");
    
        var group = await Group.GetAsync(name);
        
        if (group is null)
            throw new HException($"Group {name} not found", ExceptionType.NotFound);
        
        var response = GroupResponseWithRoles.FromGroup(group);

        return Ok(response);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(GroupResponseWithRoles), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request)
    {
        await using var context = AuthContext.Get("Group.Create");
        
        var group = await Group.GetAsync(request.Name);

        if (group is not null)
            throw new HException("There is already a group with this name", ExceptionType.InvalidOperation, HttpStatusCode.Conflict);
        
        if (request.Roles is {Count: 0})
            throw new HException("To create a group, at least one valid role must be provided", ExceptionType.MissingParams, HttpStatusCode.BadRequest);
        
        var roles = await Role.SearchAsync(names: request.Roles);
        
        if (roles is {Count: 0})
            throw new HException("No valid roles were found", ExceptionType.NotFound, HttpStatusCode.NotFound);
        
        group = await Group.CreateAsync(request.Name, request.Description, roles);
        
        var response = GroupResponseWithRoles.FromGroup(group);
        await context.SaveChangesAsync();

        return Created(string.Empty, response);
    }
    
    [HttpDelete]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromForm] string name)
    {
        await using var context = AuthContext.Get("Group.Delete");

        var group = await Group.GetAsync(name);

        if (group is null)
            throw new HException($"Group {name} not found", ExceptionType.NotFound);
        
        await group.DeleteAsync();
        await context.SaveChangesAsync();

        return Ok(true);
    }

    [HttpPut]
    [ProducesResponseType(typeof(GroupResponseWithRoles), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromQuery] string name, [FromBody] UpdateGroupRequest request)
    {
        await using var context = AuthContext.Get("Group.Update");

        var group = await Group.GetAsync(name);

        if (group is null)
            throw new HException($"Group {name} not found", ExceptionType.NotFound);

        group.Update(request.Name, request.Description);
        await context.SaveChangesAsync();
        
        var response = GroupResponseWithRoles.FromGroup(group);

        return Ok(response);
    }
    
    [HttpPost, Route("HasRole")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> HasRole([FromForm] string groupName, [FromForm] string roleName)
    {
        await using var context = AuthContext.Get("Group.HasRole");

        var group = await Group.GetAsync(groupName);
        
        if (group is null)
            throw new HException($"Group {groupName} not found", ExceptionType.NotFound);

        var hasRole = await group.HasRoleAsync(roleName);

        return Ok(hasRole);
    }
    
    [HttpPost, Route("AddRole")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddRole([FromForm] string groupName, [FromForm] string roleName)
    {
        await using var context = AuthContext.Get("Group.AddRole");

        var group = await Group.GetAsync(groupName);
        var role = await Role.GetAsync(roleName);

        if (group is null)
            throw new HException($"Group {groupName} not found", ExceptionType.NotFound);
        
        if (role is null)
            throw new HException($"Role {roleName} not found", ExceptionType.NotFound);
        
        var groupRole = await GroupRole.GetAsync(group.Id, role.Id);
        
        if (groupRole is not null)
            throw new HException("Group already has this role", ExceptionType.InvalidOperation);
        
        await GroupRole.AddAsync(group, role);
        await context.SaveChangesAsync();

        return Ok(true);
    }
    
    [HttpPost, Route("RemoveRole")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveRole([FromForm] string groupName, [FromForm] string roleName)
    {
        await using var context = AuthContext.Get("Group.RemoveRole");

        var group = await Group.GetAsync(groupName);
        var role = await Role.GetAsync(roleName);

        if (group is null)
            throw new HException($"Group {groupName} not found", ExceptionType.NotFound);
        
        if (role is null)
            throw new HException($"Role {roleName} not found", ExceptionType.NotFound);
        
        var groupRole = await GroupRole.GetAsync(group.Id, role.Id);
        
        if (groupRole is null)
            throw new HException("Group does not have this role", ExceptionType.NotFound);
        
        await GroupRole.DeleteAsync(group.Id, role.Id);
        await context.SaveChangesAsync();

        return Ok(true);
    }
}