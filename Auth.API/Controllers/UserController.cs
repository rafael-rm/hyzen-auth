using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Domain.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserApplicationService _userApplicationService;
    
    public UserController(IUserApplicationService userApplicationService)
    {
        _userApplicationService = userApplicationService;
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddAsync(CreateUserDto userDto)
    {
        try
        {
            await _userApplicationService.AddAsync(userDto);
            return Created("", userDto);
        }
        catch (UserAlreadyExistsException ex)
        {
            return Conflict(ex.Message);
        }
    }
    
}