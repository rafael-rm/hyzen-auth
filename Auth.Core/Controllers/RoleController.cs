using Auth.Core.DTO.Request.Role;
using Auth.Core.DTO.Response.Role;
using Auth.Core.Infrastructure;
using Auth.Core.Models;
using Hyzen.SDK.Exception;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Core.Controllers;

[ApiController]
[Route("api/v1/Role")]
public class RoleController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] string name)
    {
        await using var context = AuthContext.Get("Role.Get");
    
        var role = await Role.GetAsync(name);

        if (role is null)
            throw new HException($"Role {name} not found", ExceptionType.NotFound);
    
        var response = RoleResponse.FromRole(role);

        return Ok(response);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
    {
        await using var context = AuthContext.Get("Role.Create");

        var role = await Role.GetAsync(request.Name);

        if (role is not null)
            throw new HException("There is already a role with this name", ExceptionType.InvalidOperation);

        role = await Role.CreateAsync(request.Name, request.Description);

        var response = RoleResponse.FromRole(role);
        await context.SaveChangesAsync();

        return Ok(response);
    }
    
    [HttpDelete]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> Delete([FromForm] string name)
    {
        await using var context = AuthContext.Get("Role.Delete");
    
        var role = await Role.GetAsync(name);

        if (role is null)
            throw new HException($"Role {name} not found", ExceptionType.NotFound);
    
        await role.DeleteAsync();
        
        await context.SaveChangesAsync();

        return Ok(true);
    }

    [HttpPut]
    [ProducesResponseType(typeof(RoleResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update([FromQuery] string name, [FromBody] UpdateRoleRequest request)
    {
        await using var context = AuthContext.Get("Role.Update");

        var role = await Role.GetAsync(name);

        if (role is null)
            throw new HException($"Role {name} not found", ExceptionType.NotFound);
        
        role.Update(request.Name, request.Description);

        var response = RoleResponse.FromRole(role);
        await context.SaveChangesAsync();

        return Ok(response);
    }
}