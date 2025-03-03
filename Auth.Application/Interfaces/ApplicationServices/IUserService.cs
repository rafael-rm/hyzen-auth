using Auth.Application.Common;
using Auth.Application.DTOs.Request;

namespace Auth.Application.Interfaces.ApplicationServices;

public interface IUserService
{
    Task<Result> CreateAsync(CreateUserRequest user);
    Task<Result> GetByGuidAsync(Guid userId);
    Task<Result> GetByEmailAsync(string email);
    Task<Result> DeleteAsync(Guid userId);
    Task<Result> UpdateRolesAsync(Guid userId, List<string> roleIds);
}