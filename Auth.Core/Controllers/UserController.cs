using Auth.Core.DTO.Request.User;
using Auth.Core.DTO.Response.User;
using Auth.Core.Infrastructure;
using Auth.Core.Models;
using Hyzen.SDK.Authentication;
using Hyzen.SDK.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Core.Controllers;

[ApiController]
[Route("api/v1/User")]
public class UserController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] Guid id)
    {
        await using var context = AuthContext.Get("User.Get");
        
        var user = await Models.User.GetAsync(id);

        if (user is null)
            return NotFound("User not found");
        
        var response = UserResponse.FromUser(user);

        return Ok(response);
    }
    
    [HttpPost, AllowAnonymous]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        await using var context = AuthContext.Get("User.Create");
        
        var user = await Models.User.GetAsync(request.Email);

        if (user is not null)
            throw new HException("There is already a user with this email", ExceptionType.InvalidOperation);

        user = await Models.User.CreateAsync(request);
        
        var response = UserResponse.FromUser(user);
        await context.SaveChangesAsync();

        return Ok(response);
    }
    
    [HttpDelete]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromForm] Guid id)
    {
        await using var context = AuthContext.Get("User.Delete");
        
        var user = await Models.User.GetAsync(id);

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);
        
        user.Delete();
        
        await context.SaveChangesAsync();

        return Ok(true);
    }
    
    [HttpPut]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromForm] Guid userGuid, [FromBody] UpdateUserRequest request)
    {
        await using var context = AuthContext.Get("User.Update");
        
        var user = await Models.User.GetAsync(userGuid);;

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);
        
        user.Update(request);
        
        var response = UserResponse.FromUser(user);
        await context.SaveChangesAsync();

        return Ok(response);
    }
    
    [HttpPost, Route("HasRole")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> HasRole([FromForm] Guid userGuid, [FromForm] string roleName)
    {
        await using var context = AuthContext.Get("Role.HasRole");

        var user = await Models.User.GetAsync(userGuid);

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);

        var hasRole = await user.HasRole(roleName);

        return Ok(hasRole);
    }
    
    [HttpPost, Route("AddRole")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddRole([FromForm] Guid userGuid, [FromForm] string roleName)
    {
        await using var context = AuthContext.Get("User.AddRole");

        var user = await Models.User.GetAsync(userGuid);

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);

        var role = await Role.GetAsync(roleName);
        
        if (role is null)
            throw new HException("Role not found", ExceptionType.NotFound);

        if (await user.HasRole(roleName))
            throw new HException("The user already has this role", ExceptionType.InvalidOperation);

        await UserRole.Add(user.Id, role.Id);
        
        await context.SaveChangesAsync();

        return Ok(true);
    }
    
    [HttpPost, Route("RemoveRole")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveRole([FromForm] Guid userGuid, [FromForm] string roleName)
    {
        await using var context = AuthContext.Get("User.RemoveRole");

        var user = await Models.User.GetAsync(userGuid);

        if (user is null)
            throw new HException("User not found", ExceptionType.NotFound);

        var role = await Role.GetAsync(roleName);
        
        if (role is null)
            throw new HException("Role not found", ExceptionType.NotFound);
        
        if (!await user.HasRole(roleName))
            throw new HException("User does not have this role", ExceptionType.InvalidOperation);

        var wasRemoved = await UserRole.DeleteAsync(user.Id, role.Id);
        
        if (!wasRemoved)
            throw new HException("Role could not be removed", ExceptionType.InvalidOperation);
        
        await context.SaveChangesAsync();

        return Ok(true);
    }
    
    [HttpPost, Route("AddGroup")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddGroup([FromForm] Guid userGuid, [FromForm] string groupName)
    {
        await HyzenAuth.EnsureRole("hyzen_auth:user:manage_group");
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
    
    [HttpPost, Route("RemoveGroup")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveGroup([FromForm] Guid userGuid, [FromForm] string groupName)
    {
        await HyzenAuth.EnsureRole("hyzen_auth:user:manage_group");
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