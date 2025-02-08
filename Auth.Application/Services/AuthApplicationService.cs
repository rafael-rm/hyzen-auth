using Auth.Application.DTOs.Response;
using Auth.Application.Exceptions;
using Auth.Application.Interfaces;
using Auth.Application.Mappers.Interfaces;
using Auth.Domain.Core.Interfaces.Services;
using Auth.Domain.Entities;

namespace Auth.Application.Services;

public class AuthApplicationService : IAuthApplicationService
{
    private readonly IUserService _userService;
    private readonly IAuthService _authService;
    private readonly IHashService _hashService;
    private readonly ITokenService _tokenService;
    private readonly IMapper<User, UserResponse> _mapperDto;
    
    public AuthApplicationService(IUserService userService, IAuthService authService, IHashService hashService, ITokenService tokenService, IMapper<User, UserResponse> mapperDto)
    {
        _userService = userService;
        _hashService = hashService;
        _authService = authService;
        _tokenService = tokenService;
        _mapperDto = mapperDto;
    }
    
    public async Task<LoginResponse> LoginAsync(string email, string password)
    {
        var token = await _authService.LoginAsync(email, password);
        
        return new LoginResponse
        {
            Token = token,
        };
    }

    public async Task VerifyAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidTokenException();
        
        if (!await _tokenService.VerifyAsync(token))
            throw new InvalidTokenException();
    }
}