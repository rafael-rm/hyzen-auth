using Auth.Application.DTOs;

namespace Auth.Application.Interfaces;

public interface IUserApplicationService
{
    Task AddAsync(CreateUserDto user);
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto?> GetByGuidAsync(Guid guid);
    Task<UserDto?> GetByEmailAsync(string email);
    Task UpdateAsync(UserDto user);
    Task DeleteAsync(UserDto user);
}