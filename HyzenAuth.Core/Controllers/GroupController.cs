using HyzenAuth.Core.DTO.Request.Group;
using HyzenAuth.Core.DTO.Response.Group;
using HyzenAuth.Core.DTO.Response.Role;
using HyzenAuth.Core.Infrastructure;
using HyzenAuth.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyzenAuth.Core.Controllers;

[ApiController]
[Route("api/v1/Group")]
[Authorize]
public class GroupController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string name)
    {
        await using var context = AuthContext.Get("Group.Get");
    
        var group = await Group.GetAsync(name);
        
        if (group is null)
            return NotFound("Group not found");
        
        var response = GroupResponse.FromGroup(group);

        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request)
    {
        await using var context = AuthContext.Get("Group.Create");
        
        var group = await Group.GetAsync(request.Name);

        if (group is not null)
            return Conflict("There is already a group with this name");
        
        if (request.Roles is {Count: 0})
            return BadRequest("Roles not provided");
        
        var roles = await Role.SearchAsync(names: request.Roles);
        
        if (roles is {Count: 0})
            return NotFound("To create a group, at least one valid role must be provided");
        
        group = await Group.CreateAsync(request.Name, roles);
        
        var response = GroupResponse.FromGroup(group);
        await context.SaveChangesAsync();

        return Ok(response);
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete([FromForm] string name)
    {
        await using var context = AuthContext.Get("Group.Delete");

        var group = await Group.GetAsync(name);

        if (group is null)
            return NotFound("Group not found");
        
        await group.DeleteAsync();
        await context.SaveChangesAsync();

        return Ok("Group deleted");
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromForm] string oldName, [FromForm] string newName)
    {
        await using var context = AuthContext.Get("Group.Update");

        var group = await Group.GetAsync(oldName);

        if (group is null)
            return NotFound("Group not found");
        
        group.Update(newName);
        await context.SaveChangesAsync();

        return Ok("Group updated");
    }
    
    [HttpPost, Route("HasRole")]
    public async Task<IActionResult> HasRole([FromForm] string groupName, [FromForm] string roleName)
    {
        await using var context = AuthContext.Get("Group.HasRole");

        var group = await Group.GetAsync(groupName);
        
        if (group is null)
            return NotFound("Group not found");

        var hasRole = await group.HasRoleAsync(roleName);

        return Ok(hasRole);
    }
    
    [HttpPost, Route("AddRole")]
    public async Task<IActionResult> AddRole([FromForm] string groupName, [FromForm] string roleName)
    {
        await using var context = AuthContext.Get("Group.AddRole");

        var group = await Group.GetAsync(groupName);
        var role = await Role.GetAsync(roleName);

        if (group is null)
            return NotFound("Group not found");
        
        if (role is null)
            return NotFound("Role not found");
        
        var groupRole = await GroupRole.GetAsync(group.Id, role.Id);
        
        if (groupRole is not null)
            return Conflict("Group already has this role");
        
        await GroupRole.AddAsync(group, role);
        await context.SaveChangesAsync();

        return Ok("Role added to group");
    }
    
    [HttpPost, Route("RemoveRole")]
    public async Task<IActionResult> RemoveRole([FromForm] string groupName, [FromForm] string roleName)
    {
        await using var context = AuthContext.Get("Group.RemoveRole");

        var group = await Group.GetAsync(groupName);
        var role = await Role.GetAsync(roleName);

        if (group is null)
            return NotFound("Group not found");
        
        if (role is null)
            return NotFound("Role not found");
        
        var groupRole = await GroupRole.GetAsync(group.Id, role.Id);
        
        if (groupRole is null)
            return NotFound("Group does not have this role");
        
        await GroupRole.DeleteAsync(group.Id, role.Id);
        await context.SaveChangesAsync();

        return Ok("Role removed from group");
    }
}