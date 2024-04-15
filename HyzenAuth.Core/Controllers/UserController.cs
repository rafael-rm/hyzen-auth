using HyzenAuth.Core.DTO.Request.User;
using HyzenAuth.Core.DTO.Response.User;
using HyzenAuth.Core.Infrastructure;
using HyzenAuth.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyzenAuth.Core.Controllers;

[ApiController]
[Route("api/v1/User")]
[Authorize]
public class UserController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid id)
    {
        await using var context = AuthContext.Get("User.Get");
        
        var user = await Models.User.GetAsync(id);

        if (user is null)
            return NotFound("User not found");
        
        var response = UserResponse.FromUser(user);

        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        await using var context = AuthContext.Get("User.Create");
        
        var user = (await Models.User.SearchAsync(email: request.Email)).FirstOrDefault();

        if (user is not null)
            return Conflict("There is already a registered user with this email");

        user = await Models.User.CreateAsync(request);
        
        var response = UserResponse.FromUser(user);
        await context.SaveChangesAsync();

        return Ok(response);
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete([FromForm] Guid id)
    {
        await using var context = AuthContext.Get("User.Delete");
        
        var user = await Models.User.GetAsync(id);

        if (user is null)
            return NotFound("User not found");
        
        user.Delete();
        
        await context.SaveChangesAsync();

        return Ok("User deleted");
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromForm] Guid userGuid, [FromBody] UpdateUserRequest request)
    {
        await using var context = AuthContext.Get("User.Update");
        
        var user = await Models.User.GetAsync(userGuid);;

        if (user is null)
            return NotFound("User not found");
        
        user.Update(request);
        
        var response = UserResponse.FromUser(user);
        await context.SaveChangesAsync();

        return Ok(response);
    }
    
    [HttpPost, Route("HasRole")]
    public async Task<IActionResult> HasRole([FromForm] Guid userGuid, [FromForm] string roleName)
    {
        await using var context = AuthContext.Get("Role.HasRole");

        var user = await Models.User.GetAsync(userGuid);

        if (user is null)
            return NotFound("User not found");

        var hasRole = await user.HasRole(roleName);

        return Ok(hasRole);
    }
    
    [HttpPost, Route("AddRole")]
    public async Task<IActionResult> AddRole([FromForm] Guid userGuid, [FromForm] string roleName)
    {
        await using var context = AuthContext.Get("User.AddRole");

        var user = await Models.User.GetAsync(userGuid);

        if (user is null)
            return NotFound("User not found");

        var role = await Role.GetAsync(roleName);
        
        if (role is null)
            return NotFound("Role not found");

        if (await user.HasRole(roleName))
            return Conflict("The user already has this role");

        await UserRole.Add(user, role);
        
        await context.SaveChangesAsync();

        return Ok("Role added to user");
    }
    
    [HttpPost, Route("RemoveRole")]
    public async Task<IActionResult> RemoveRole([FromForm] Guid userGuid, [FromForm] string roleName)
    {
        await using var context = AuthContext.Get("User.RemoveRole");

        var user = await Models.User.GetAsync(userGuid);

        if (user is null)
            return NotFound("User not found");

        var role = await Role.GetAsync(roleName);
        
        if (role is null)
            return NotFound("Role not found");
        
        if (!await user.HasRole(roleName))
            return Conflict("User does not have this role");

        var wasRemoved = await UserRole.DeleteAsync(user.Id, role.Id);
        
        if (!wasRemoved)
            return Conflict("The user is associated with a group that contains this permission");
        
        await context.SaveChangesAsync();

        return Ok("Role removed from user");
    }
    
    [HttpPost, Route("AddGroup")]
    public async Task<IActionResult> AddGroup([FromForm] Guid userGuid, [FromForm] string groupName)
    {
        await using var context = AuthContext.Get("User.AddGroup");

        var user = await Models.User.GetAsync(userGuid);

        if (user is null)
            return NotFound("User not found");

        var group = await Group.GetAsync(groupName);
        
        if (group is null)
            return NotFound("Group not found");

        if (await user.HasGroup(groupName))
            return Conflict("The user already has this group");

        await UserGroup.Add(user, group);
        
        await context.SaveChangesAsync();

        return Ok("Group added to user");
    }
    
    [HttpPost, Route("RemoveGroup")]
    public async Task<IActionResult> RemoveGroup([FromForm] Guid userGuid, [FromForm] string groupName)
    {
        await using var context = AuthContext.Get("User.RemoveGroup");

        var user = await Models.User.GetAsync(userGuid);

        if (user is null)
            return NotFound("User not found");

        var group = await Group.GetAsync(groupName);
        
        if (group is null)
            return NotFound("Group not found");
        
        if (!await user.HasGroup(groupName))
            return Conflict("User does not have this group");

        await UserGroup.DeleteAsync(user.Id, group.Id);
        
        await context.SaveChangesAsync();

        return Ok("Group removed from user");
    }
}