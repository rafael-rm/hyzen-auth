using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Domain.Exceptions.Role;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Services;

public class RoleService : IRoleService
{
    private readonly IAuthDbContext _authDbContext;
    
    public RoleService(IAuthDbContext authDbContext)
    {
        _authDbContext = authDbContext;
    }

    public async Task<RoleResponse> CreateAsync(CreateRoleRequest request)
    {
        // TODO: Add validation
        
        var role = new Role(request.Key, request.Name, request.Description);
        
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

    public async Task<RoleResponse> GetByKeyAsync(string key)
    {
        var role = await _authDbContext.Roles.FirstOrDefaultAsync(s => s.Key == key);
        
        if (role is null)
            throw new RoleNotFoundException(key);
        
        return RoleResponse.FromEntity(role);
    }

    public async Task DeleteAsync(string key)
    {
        var role = await _authDbContext.Roles.FirstOrDefaultAsync(s => s.Key == key);

        if (role is null)
            throw new RoleNotFoundException(key);

        _authDbContext.Roles.Remove(role);
        await _authDbContext.SaveChangesAsync();
    }
}