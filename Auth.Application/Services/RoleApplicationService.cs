using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Domain.Exceptions.Role;
using Auth.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Services;

public class RoleApplicationService : IRoleApplicationService
{
    private readonly IAuthDbContext _authDbContext;
    private readonly IRoleService _roleService;
    
    public RoleApplicationService(IAuthDbContext authDbContext, IRoleService roleService)
    {
        _authDbContext = authDbContext;
        _roleService = roleService;
    }

    public async Task<RoleResponse> CreateAsync(CreateRoleRequest createRoleRequest)
    {
        var role = new Role(createRoleRequest.Name, createRoleRequest.Description);
        
        await _authDbContext.Roles.AddAsync(role);
        await _authDbContext.SaveChangesAsync();
        
        return RoleResponse.FromEntity(role);
    }

    public async Task<RoleResponse> GetByGuidAsync(Guid groupId)
    {
        var role = await _authDbContext.Roles.FirstOrDefaultAsync(s => s.Guid == groupId);

        if (role is null)
            throw new RoleNotFoundException(groupId);
        
        return RoleResponse.FromEntity(role);
    }

    public async Task<RoleResponse> GetByNameAsync(string name)
    {
        var role = await _authDbContext.Roles.FirstOrDefaultAsync(s => s.Name == name);
        
        if (role is null)
            throw new RoleNotFoundException(name);
        
        return RoleResponse.FromEntity(role);
    }

    public async Task DeleteAsync(string name)
    {
        var role = await _authDbContext.Roles.FirstOrDefaultAsync(s => s.Name == name);

        if (role is null)
            throw new RoleNotFoundException(name);

        _authDbContext.Roles.Remove(role);
        await _authDbContext.SaveChangesAsync();
    }
}