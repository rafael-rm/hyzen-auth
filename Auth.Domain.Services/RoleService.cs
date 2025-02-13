using Auth.Domain.Entities;
using Auth.Domain.Exceptions.Role;
using Auth.Domain.Interfaces.Repositories;
using Auth.Domain.Interfaces.Services;

namespace Auth.Domain.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    
    public RoleService(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }
    
    public async Task<Role> CreateAsync(Role role)
    {
        var existingRole = await _roleRepository.GetByNameAsync(role.Name);
        
        if (existingRole != null)
            throw new RoleAlreadyExistsException(existingRole.Name);
        
        await _roleRepository.AddAsync(role);
        
        return role;
    }

    public async Task<Role?> GetByGuidAsync(Guid roleId)
    {
        return await _roleRepository.GetByGuidAsync(roleId);
    }

    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _roleRepository.GetByNameAsync(name);
    }

    public Task DeleteAsync(Role role)
    {
        _roleRepository.Delete(role);
        
        return Task.CompletedTask;
    }
}