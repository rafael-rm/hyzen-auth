using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;

namespace Auth.Application.Interfaces;

public interface IUserApplicationService
{
    Task<UserResponse> CreateAsync(CreateUserRequest user);
    Task<UserResponse> GetByGuidAsync(Guid userId);
    Task<UserResponse> GetByEmailAsync(string email);
    Task DeleteAsync(Guid userId);
}