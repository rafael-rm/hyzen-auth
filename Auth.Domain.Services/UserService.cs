using Auth.Domain.Entities;
using Auth.Domain.Exceptions;
using Auth.Domain.Interfaces.Repositories;
using Auth.Domain.Interfaces.Services;

namespace Auth.Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<User> CreateAsync(User user)
    {
        var existingUser = await _userRepository.GetByEmailAsync(user.Email);
        
        if (existingUser != null)
            throw new UserAlreadyExistsException(existingUser.Email);
        
        await _userRepository.AddAsync(user);
        
        return user;
    }
    
    public async Task<User?> GetByGuidAsync(Guid userId)
    {
        return await _userRepository.GetByGuidAsync(userId);
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }

    public Task DeleteAsync(User user)
    {
        _userRepository.Delete(user);
        
        return Task.CompletedTask;
    }
}