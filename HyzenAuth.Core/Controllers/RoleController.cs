using HyzenAuth.Core.DTO.Request.Role;
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
    public async Task<IActionResult> Get([FromQuery] Guid term)
    {
        await using var context = AuthContext.Get("Role.Get");
    
        var role = await Role.GetAsync(term.ToString());

        if (role is null)
            return NotFound("Role not found");
    
        var response = RoleResponse.FromUser(role);

        return Ok(response);
    }
    
    [HttpPost, Authorize]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest request)
    {
        await using var context = AuthContext.Get("Role.Create");

        var role = await Role.GetAsync(request.Name);

        if (role is not null)
            return Conflict("There is already a registered role with this name");

        role = await Role.CreateAsync(request);

        var response = RoleResponse.FromUser(role);
        await context.SaveChangesAsync();

        return Ok(response);
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string term)
    {
        await using var context = AuthContext.Get("Role.Delete");
    
        var role = await Role.GetAsync(term);

        if (role is null)
            return NotFound("Role not found");
    
        role.Delete();
        
        await context.SaveChangesAsync();

        return NoContent();
    }
    
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateRoleRequest request)
    {
        await using var context = AuthContext.Get("Role.Update");
        
        var role = await Role.GetAsync(request.Id);

        if (role is null)
            return NotFound("Role not found");
    
        var response = RoleResponse.FromUser(role);
        await context.SaveChangesAsync();

        return Ok(response);
    }
}