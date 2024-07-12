using System.Net;
using Auth.Core.DTO.Request.Group;
using Auth.Core.DTO.Response.Group;
using Auth.Core.Infrastructure;
using Auth.Core.Models;
using Hyzen.SDK.Authentication;
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
        await HyzenAuth.EnsureRole("hyzen_auth:group:get");
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
        await HyzenAuth.EnsureRole("hyzen_auth:group:create");
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
        await HyzenAuth.EnsureRole("hyzen_auth:group:delete");
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
        await HyzenAuth.EnsureRole("hyzen_auth:group:update");
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
        await HyzenAuth.EnsureRole("hyzen_auth:group:has_role");
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
        await HyzenAuth.EnsureRole("hyzen_auth:group:add_role");
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
        await HyzenAuth.EnsureRole("hyzen_auth:group:remove_role");
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
    
    [HttpPost, Route("AddGroupToUser")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddGroupToUser([FromForm] Guid userGuid, [FromForm] string groupName)
    {
        await HyzenAuth.EnsureRole("hyzen_auth:user:add_group");
        await using var context = AuthContext.Get("User.AddGroup");
        
        var group = await Group.GetAsync(groupName);
        if (group is null)
            throw new HException("Group not found", ExceptionType.NotFound);

        var actor = await HyzenAuth.GetSubject();
        if (!actor.HasGroup(group.Name))
            throw new HException("You do not have permission to add this group", ExceptionType.InvalidOperation);
        
        var user = await Models.User.GetAsync(userGuid);
        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);

        if (await user.HasGroup(groupName))
            throw new HException("The user already has this group", ExceptionType.InvalidOperation);

        await UserGroup.AddAsync(user, group);
        
        await context.SaveChangesAsync();

        return Ok(true);
    }
    
    [HttpPost, Route("RemoveGroupToUser")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveGroupToUser([FromForm] Guid userGuid, [FromForm] string groupName)
    {
        await HyzenAuth.EnsureRole("hyzen_auth:user:remove_group");
        await using var context = AuthContext.Get("User.RemoveGroup");

        var user = await Models.User.GetAsync(userGuid);

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);

        var group = await Group.GetAsync(groupName);
        
        if (group is null)
            throw new HException("Group not found", ExceptionType.NotFound);
        
        if (!await user.HasGroup(groupName))
            throw new HException("User does not have this group", ExceptionType.InvalidOperation);

        await UserGroup.DeleteAsync(user.Id, group.Id);
        
        await context.SaveChangesAsync();

        return Ok(true);
    }
}