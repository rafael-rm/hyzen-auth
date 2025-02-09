using Auth.Application.DTOs.Response;
using Auth.Application.Exceptions;
using Auth.Application.Interfaces;
using Auth.Domain.Interfaces.Services;

namespace Auth.Application.Services;

public class AuthApplicationService : IAuthApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;
    
    public AuthApplicationService(IUnitOfWork unitOfWork, IAuthService authService, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
        _tokenService = tokenService;
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