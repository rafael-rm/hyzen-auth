using Auth.Application.Common;
using Auth.Application.DTOs.Request;

namespace Auth.Application.Interfaces.Application;

public interface IRoleService
{
    Task<Result> CreateAsync(CreateRoleRequest request);
    Task<Result> GetByKeyAsync(string key);
    Task<Result> DeleteAsync(string key);
}