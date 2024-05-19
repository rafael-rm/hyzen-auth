using Hyzen.SDK.Exception;
using HyzenAuth.Core.DTO.Response.Role;
using HyzenAuth.Core.Infrastructure;
using HyzenAuth.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyzenAuth.Core.Controllers;

[ApiController]
[Route("api/v1/Role")]
[Authorize]
public class RoleController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string name)
    {
        await using var context = AuthContext.Get("Role.Get");
    
        var role = await Role.GetAsync(name);

        if (role is null)
            throw new HException("Role not found", ExceptionType.NotFound);
    
        var response = RoleResponse.FromRole(role);

        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] string name)
    {
        await using var context = AuthContext.Get("Role.Create");

        var role = await Role.GetAsync(name);

        if (role is not null)
            throw new HException("There is already a role with this name", ExceptionType.InvalidOperation);

        role = await Role.CreateAsync(name);

        var response = RoleResponse.FromRole(role);
        await context.SaveChangesAsync();

        return Ok(response);
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete([FromForm] string name)
    {
        await using var context = AuthContext.Get("Role.Delete");
    
        var role = await Role.GetAsync(name);

        if (role is null)
            throw new HException("Role not found", ExceptionType.NotFound);
    
        await role.DeleteAsync();
        
        await context.SaveChangesAsync();

        return Ok("Role deleted");
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromForm] string oldName, [FromForm] string newName)
    {
        await using var context = AuthContext.Get("Role.Update");

        var role = await Role.GetAsync(oldName);

        if (role is null)
            throw new HException("Role not found", ExceptionType.NotFound);
        
        role.Update(newName);

        var response = RoleResponse.FromRole(role);
        await context.SaveChangesAsync();

        return Ok(response);
    }
}