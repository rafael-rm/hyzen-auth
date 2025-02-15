using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;

namespace Auth.Application.Interfaces;

public interface IRoleService
{
    Task<RoleResponse> CreateAsync(CreateRoleRequest request);
    Task<RoleResponse> GetByGuidAsync(Guid roleId);
    Task<RoleResponse> GetByKeyAsync(string key);
    Task DeleteAsync(string key);
}