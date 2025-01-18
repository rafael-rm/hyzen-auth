using Auth.Domain.Core.Exceptions;
using Auth.Domain.Core.Interfaces.Repositories;
using Auth.Domain.Core.Interfaces.Services;
using Auth.Domain.Entities;

namespace Auth.Domain.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task CreateAsync(User user)
    {
        var existingUser = await _userRepository.GetByEmailAsync(user.Email);
        
        if (existingUser != null)
            throw new UserAlreadyExistsException(existingUser.Email);
        
        await _userRepository.AddAsync(user);
    }
    
    public async Task<User?> GetByGuidAsync(Guid guid)
    {
        return await _userRepository.GetByGuidAsync(guid);
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userRepository.GetByEmailAsync(email);
    }
}