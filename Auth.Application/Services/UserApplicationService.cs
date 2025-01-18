using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Application.Mappers.Interfaces;
using Auth.Domain.Core.Interfaces.Services;
using Auth.Domain.Entities;

namespace Auth.Application.Services;

public class UserApplicationService : IUserApplicationService
{
    private readonly IUserService _userService;
    private readonly IMapper<UserDto, User> _mapperEntity;
    private readonly IMapper<CreateUserDto, User> _mapperCreate;
    
    public UserApplicationService(IUserService userService, IMapper<UserDto, User> mapperEntity, IMapper<CreateUserDto, User> mapperCreate)
    {
        _userService = userService;
        _mapperEntity = mapperEntity;
        _mapperCreate = mapperCreate;
    }
    
    public async Task AddAsync(CreateUserDto createUserDto)
    {
        var user = _mapperCreate.Map(createUserDto);
        
        user.Guid = Guid.NewGuid();
        user.IsActive = true;
        
        await _userService.AddAsync(user);
    }

    public Task<UserDto?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto?> GetByGuidAsync(Guid guid)
    {
        throw new NotImplementedException();
    }

    public Task<UserDto?> GetByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(UserDto user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(UserDto user)
    {
        throw new NotImplementedException();
    }
}