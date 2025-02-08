using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Application.Mappers.Interfaces;
using Auth.Domain.Entities;
using Auth.Domain.Exceptions;
using Auth.Domain.Interfaces.Repositories;
using Auth.Domain.Interfaces.Services;

namespace Auth.Application.Services;

public class UserApplicationService : IUserApplicationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserService _userService;
    private readonly IHashService _hashService;
    private readonly IMapper<User, UserResponse> _mapperDto;
    private readonly IMapper<CreateUserRequest, User> _mapperCreate;
    
    public UserApplicationService(IUnitOfWork unitOfWork, IUserService userService, IHashService hashService, IMapper<User, UserResponse> mapperDto, IMapper<CreateUserRequest, User> mapperCreate)
    {
        _unitOfWork = unitOfWork;
        _userService = userService;
        _hashService = hashService;
        _mapperDto = mapperDto;
        _mapperCreate = mapperCreate;
    }
    
    public async Task<UserResponse> CreateAsync(CreateUserRequest createUserRequest)
    {
        var user = _mapperCreate.Map(createUserRequest);
        
        user.Guid = Guid.NewGuid();
        user.IsActive = true;
        user.Password = _hashService.Hash(createUserRequest.Password);
        
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