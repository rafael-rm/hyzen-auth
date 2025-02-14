using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Domain.Exceptions.User;
using Auth.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Services;

public class UserService : IUserService
{
    private readonly IAuthDbContext _authDbContext;
    private readonly IHashService _hashService;
    
    public UserService(IAuthDbContext authDbContext, IHashService hashService)
    {
        _authDbContext = authDbContext;
        _hashService = hashService;
    }
    
    public async Task<UserResponse> CreateAsync(CreateUserRequest createUserRequest)
    {
        var user = new User(createUserRequest.Name, createUserRequest.Email, _hashService.Hash(createUserRequest.Password));
        
        await _authDbContext.Users.AddAsync(user);
        await _authDbContext.SaveChangesAsync();
        
        return UserResponse.FromEntity(user);
    }

    public async Task<UserResponse> GetByGuidAsync(Guid userId)
    {
        var user = await _authDbContext.Users.FirstOrDefaultAsync(s => s.Guid == userId);

        if (user is null)
            throw new UserNotFoundException(userId);
        
        return UserResponse.FromEntity(user);
    }

    public async Task<UserResponse> GetByEmailAsync(string email)
    {
        var user = await _authDbContext.Users.FirstOrDefaultAsync(s => s.Email == email);
        
        if (user is null)
            throw new UserNotFoundException(email);
        
        return UserResponse.FromEntity(user);
    }

    public async Task DeleteAsync(Guid userId)
    {
        var user = await _authDbContext.Users.FirstOrDefaultAsync(s => s.Guid == userId);

        if (user is null)
            throw new UserNotFoundException(userId);

        _authDbContext.Users.Remove(user);
        await _authDbContext.SaveChangesAsync();
    }
}