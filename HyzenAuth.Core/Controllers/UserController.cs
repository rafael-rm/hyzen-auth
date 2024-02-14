using HyzenAuth.Core.DTO.Request;
using HyzenAuth.Core.DTO.Response;
using HyzenAuth.Core.Infrastructure;
using HyzenAuth.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace HyzenAuth.Core.Controllers;

[ApiController]
[Route("api/v1/User")]
public class UserController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] string id)
    {
        await using var context = AuthContext.Get("User.Get");
        
        var user = await Models.User.GetAsync(id);

        if (user is null)
            return NotFound("User not found");
        
        var response = UserResponse.FromUser(user);
        await context.SaveChangesAsync();

        return Ok(response);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        await using var context = AuthContext.Get("User.Create");
        
        var user = (await Models.User.SearchAsync(email: request.Email)).FirstOrDefault();

        if (user is not null)
            return Conflict("There is already a registered user with this email");

        request.Password = PasswordHelper.HashPassword(request.Password);
        
        user = await Models.User.CreateAsync(request);
        
        var response = UserResponse.FromUser(user);
        await context.SaveChangesAsync();

        return Ok(response);
    }
    
    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string id)
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
    public async Task<IActionResult> Update([FromBody] UpdateUserRequest request)
    {
        await using var context = AuthContext.Get("User.Update");
        
        var user = await Models.User.GetAsync(request.Id);;

        if (user is null)
            return NotFound("User not found");

        request.Password = PasswordHelper.HashPassword(request.Password);
        
        user.Update(request);
        
        var response = UserResponse.FromUser(user);
        await context.SaveChangesAsync();

        return Ok(response);
    }
}