using Auth.Application.Common;
using Auth.Application.DTOs.Request;

namespace Auth.Application.Interfaces.ApplicationServices;

public interface IRoleService
{
    Task<Result> CreateAsync(CreateRoleRequest request);
    Task<Result> GetByKeyAsync(string key);
    Task<Result> DeleteAsync(string key);
}