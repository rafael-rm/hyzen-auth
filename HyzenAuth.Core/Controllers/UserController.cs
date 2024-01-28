using HyzenAuth.Core.DTO.Request;
using HyzenAuth.Core.DTO.Response;
using HyzenAuth.Core.Infrastructure;
using HyzenAuth.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace HyzenAuth.Core.Controllers;

[ApiController]
[Route("api/v1/Auth")]
public class UserController : ControllerBase
{
    [HttpPost, Route("Create")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        await using var context = AuthContext.Get("User.Create");
        
        var user = (await Models.User.SearchAsync(email: request.Email)).FirstOrDefault();

        if (user is not null)
            throw new Exception("There is already a registered user with this email");
        
        // TODO: Encrypt password before saving to database

        user = await Models.User.CreateAsync(request);
        
        var response = CreateUserResponse.FromUser(user);
        
        await context.SaveChangesAsync();

        return Ok(response);
    }
}