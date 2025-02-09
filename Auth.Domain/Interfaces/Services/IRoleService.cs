using Auth.Domain.Entities;

namespace Auth.Domain.Interfaces.Services;

public interface IRoleService
{
    Task<Role> CreateAsync(Role role);
    Task<Role?> GetByGuidAsync(Guid roleId);
    Task<Role?> GetByNameAsync(string name);
    Task DeleteAsync(Role role);
}