using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;

namespace Auth.Application.Interfaces;

public interface IRoleService
{
    Task<RoleResponse> CreateAsync(CreateRoleRequest createRoleRequest);
    Task<RoleResponse> GetByGuidAsync(Guid roleId);
    Task<RoleResponse> GetByNameAsync(string name);
    Task DeleteAsync(string name);
}