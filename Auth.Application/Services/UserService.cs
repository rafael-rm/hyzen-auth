using Auth.Application.Common;
using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Errors;
using Auth.Application.Interfaces.ApplicationServices;
using Auth.Application.Interfaces.InfrastructureServices;
using Auth.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Services;

public class UserService(IAuthDbContext authDbContext, IHashService hashService) : IUserService
{
    public async Task<Result> CreateAsync(CreateUserRequest createUserRequest)
    {
        if (await UserExistsByEmailAsync(createUserRequest.Email))
            return Result.Failure(UserError.UserAlreadyExists);

        var hashedPassword = hashService.Hash(createUserRequest.Password);
        var user = new User(createUserRequest.Name, createUserRequest.Email, hashedPassword);

        await authDbContext.Users.AddAsync(user);
        await authDbContext.SaveChangesAsync();

        return Result.Success(UserResponse.FromEntity(user));
    }

    public async Task<Result> GetByGuidAsync(Guid userId)
    {
        var user = await authDbContext.Users.FirstOrDefaultAsync(s => s.Guid == userId);
        
        if (user is null)
            return Result.Failure(UserError.UserNotFound);
        
        return Result.Success(UserResponse.FromEntity(user));
    }

    public async Task<Result> GetByEmailAsync(string email)
    {
        var user = await authDbContext.Users.FirstOrDefaultAsync(s => s.Email == email);
        
        if (user is null)
            return Result.Failure(UserError.UserNotFound);
        
        return Result.Success(UserResponse.FromEntity(user));
    }

    public async Task<Result> DeleteAsync(Guid userId)
    {
        var user = await authDbContext.Users.FirstOrDefaultAsync(s => s.Guid == userId);

        if (user is null)
            return Result.Failure(UserError.UserNotFound);

        authDbContext.Users.Remove(user);
        await authDbContext.SaveChangesAsync();
        
        return Result.Success();
    }

    private async Task<bool> UserExistsByEmailAsync(string email)
    {
        return await authDbContext.Users.AnyAsync(s => s.Email == email);
    }
}
