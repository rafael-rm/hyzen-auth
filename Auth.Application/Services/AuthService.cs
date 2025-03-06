using Auth.Application.Common;
using Auth.Application.DTOs.Response;
using Auth.Application.Errors;
using Auth.Application.Interfaces.Application;
using Auth.Application.Interfaces.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Services;

public class AuthService(IAuthDbContext authDbContext, IHashService hashService, ITokenService tokenService) : IAuthService
{
    public async Task<Result> LoginAsync(string email, string password)
    {
        var user = await authDbContext.Users
            .Include(s => s.UserRoles)
            .ThenInclude(s => s.Role)
            .FirstOrDefaultAsync(s => s.Email == email);

        if (user is null)
            return Result.Failure(AuthError.InvalidCredentials);

        if (!hashService.Verify(password, user.Password))
            return Result.Failure(AuthError.InvalidCredentials);

        var token= tokenService.GenerateToken(user);
        
        return Result.Success(new LoginResponse
        {
            Token = token
        });
    }

    public async Task<Result> VerifyAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return Result.Failure(AuthError.InvalidToken);
        
        if (!await tokenService.VerifyAsync(token))
            return Result.Failure(AuthError.InvalidToken);
        
        return Result.Success();
    }
}