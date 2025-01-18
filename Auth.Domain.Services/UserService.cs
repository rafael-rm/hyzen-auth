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
    
    public async Task AddAsync(User user)
    {
        var existingUser = await _userRepository.GetByEmailAsync(user.Email);
        
        if (existingUser != null)
            throw new UserAlreadyExistsException(existingUser.Email);
        
        await _userRepository.AddAsync(user);
    }
}