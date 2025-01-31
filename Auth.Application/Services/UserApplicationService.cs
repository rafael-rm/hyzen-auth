using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Application.Mappers.Interfaces;
using Auth.Domain.Core.Exceptions;
using Auth.Domain.Core.Interfaces.Services;
using Auth.Domain.Entities;

namespace Auth.Application.Services;

public class UserApplicationService : IUserApplicationService
{
    private readonly IUserService _userService;
    private readonly IHashService _hashService;
    private readonly IMapper<User, UserDto> _mapperDto;
    private readonly IMapper<CreateUserDto, User> _mapperCreate;
    
    public UserApplicationService(IUserService userService, IMapper<CreateUserDto, User> mapperCreate, IHashService hashService, IMapper<User, UserDto> mapperDto)
    {
        _userService = userService;
        _hashService = hashService;
        _mapperCreate = mapperCreate;
        _mapperDto = mapperDto;
    }
    
    public async Task<UserDto> CreateAsync(CreateUserDto createUserDto)
    {
        var user = _mapperCreate.Map(createUserDto);
        
        user.Guid = Guid.NewGuid();
        user.IsActive = true;
        user.Password = _hashService.Hash(createUserDto.Password);
        
        await _userService.CreateAsync(user);
        
        return _mapperDto.Map(user);
    }

    public async Task<UserDto> GetByGuidAsync(Guid userId)
    {
        var user = await _userService.GetByGuidAsync(userId);

        if (user is null)
            throw new UserNotFoundException(userId);
        
        return _mapperDto.Map(user);
    }

    public async Task<UserDto> GetByEmailAsync(string email)
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
    }
}