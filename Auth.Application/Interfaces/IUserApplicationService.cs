using Auth.Application.DTOs;

namespace Auth.Application.Interfaces;

public interface IUserApplicationService
{
    Task CreateAsync(CreateUserDto user);
    Task<UserDto> GetByGuidAsync(Guid userId);
    Task<UserDto> GetByEmailAsync(string email);
    Task DeleteAsync(Guid userId);
}