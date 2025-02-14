using Auth.Application.DTOs.Response;
using Auth.Application.Exceptions;
using Auth.Application.Interfaces;
using Auth.Domain.Exceptions.User;
using Auth.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Services;

public class AuthService : IAuthService
{
    private readonly IAuthDbContext _authDbContext;
    private readonly IHashService _hashService;
    private readonly ITokenService _tokenService;
    
    public AuthService(IAuthDbContext authDbContext, IHashService hashService, ITokenService tokenService)
    {
        _authDbContext = authDbContext;
        _hashService = hashService;
        _tokenService = tokenService;
    }
    
    public async Task<LoginResponse> LoginAsync(string email, string password)
    {
        var user = await _authDbContext.Users.FirstOrDefaultAsync(s => s.Email == email);

        if (user is null)
            throw new AuthenticationFailedException();

        if (!_hashService.Verify(password, user.Password))
            throw new AuthenticationFailedException();

        var token= _tokenService.GenerateToken(user);
        
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