using HyzenAuth.Core.DTO.Request.User;
using HyzenAuth.Core.DTO.Response.User;
using HyzenAuth.Core.Infrastructure;
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

        return NoContent();
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
}