using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Application.Mappers.Interfaces;
using Auth.Domain.Entities;
using Auth.Domain.Exceptions.User;
using Auth.Domain.Interfaces.Services;

namespace Auth.Application.Services;

public class UserApplicationService : IUserApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly IHashService _hashService;
    private readonly IMapper<User, UserResponse> _mapperDto;
    
    public UserApplicationService(IUnitOfWork unitOfWork, IUserService userService, IHashService hashService, IMapper<User, UserResponse> mapperDto)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _hashService = hashService;
        _mapperDto = mapperDto;
    }
    
    public async Task<UserResponse> CreateAsync(CreateUserRequest createUserRequest)
    {
        var user = new User
        {
            Guid = Guid.NewGuid(),
            Name = createUserRequest.Name,
            Email = createUserRequest.Email,
            Password = _hashService.Hash(createUserRequest.Password),
            IsActive = true,
        };
        
        await _userService.CreateAsync(user);
        await _unitOfWork.CommitAsync();
        
        return _mapperDto.Map(user);
    }

    public async Task<UserResponse> GetByGuidAsync(Guid userId)
    {
        var user = await _userService.GetByGuidAsync(userId);

        if (user is null)
            throw new UserNotFoundException(userId);
        
        return _mapperDto.Map(user);
    }

    public async Task<UserResponse> GetByEmailAsync(string email)
    {
        var user = await _userService.GetByEmailAsync(email);
        
        if (user is null)
            throw new UserNotFoundException(email);
        
        return _mapperDto.Map(user);
    }

    public async Task DeleteAsync(Guid userId)
    {
        var user = await _userService.GetByGuidAsync(userId);
        
        if (user is null)
            throw new UserNotFoundException(userId);
        
        await _userService.DeleteAsync(user);
        
        await _unitOfWork.CommitAsync();
    }
}