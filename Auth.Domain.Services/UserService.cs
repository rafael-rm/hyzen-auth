using Auth.Domain.Core.Exceptions;
using Auth.Domain.Core.Interfaces.Repositories;
using Auth.Domain.Core.Interfaces.Services;
using Auth.Domain.Entities;

namespace Auth.Domain.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    
    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<User> CreateAsync(User user)
    {
        var existingUser = await _userRepository.GetByEmailAsync(user.Email);
        
        if (existingUser != null)
            throw new UserAlreadyExistsException(existingUser.Email);
        
        await _userRepository.AddAsync(user);
        await _unitOfWork.CommitAsync();
        
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

    public async Task DeleteAsync(User user)
    {
        _userRepository.Delete(user);
        await _unitOfWork.CommitAsync();
    }
}