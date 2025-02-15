using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Domain.Exceptions.Group;
using Auth.Domain.Exceptions.Role;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Services;

public class GroupService : IGroupService
{
    private readonly IAuthDbContext _authDbContext;
    
    public GroupService(IAuthDbContext authDbContext)
    {
        _authDbContext = authDbContext;
    }
    
    public async Task<GroupResponse> CreateAsync(CreateGroupRequest request)
    {
        var groupEntity = await _authDbContext.Groups
            .Include(s => s.GroupRoles)
            .ThenInclude(s => s.Role)
            .FirstOrDefaultAsync(s => s.Key == request.Key);
        
        if (groupEntity is not null)
            throw new GroupAlreadyExistsException(request.Key);
        
        var roles = await _authDbContext.Roles.Where(s => request.Roles.Contains(s.Key)).ToListAsync();
        
        var foundRoleKeys = roles.Select(r => r.Key).ToHashSet();
        var missingRoles = request.Roles.Except(foundRoleKeys).ToList();

        if (missingRoles.Count != 0)
            throw new RoleNotFoundException(missingRoles);
        
        var group = new Group(request.Key, request.Name, request.Description);

        foreach (var role in roles)
        {
            group.AddRole(role);
        }
        
        await _authDbContext.Groups.AddAsync(group);
        await _authDbContext.SaveChangesAsync();
        
        return GroupResponse.FromEntity(group);
    }

    public async Task<GroupResponse> GetByGuidAsync(Guid groupId)
    {
        var group = await _authDbContext.Groups
            .Include(s => s.GroupRoles)
            .ThenInclude(s => s.Role)
            .FirstOrDefaultAsync(s => s.Guid == groupId);

        if (group is null)
            throw new GroupNotFoundException(groupId);
        
        return GroupResponse.FromEntity(group);
    }

    public async Task<GroupResponse> GetByKeyAsync(string key)
    {
        var group = await _authDbContext.Groups
            .Include(s => s.GroupRoles)
            .ThenInclude(s => s.Role)
            .FirstOrDefaultAsync(s => s.Key == key);
        
        if (group is null)
            throw new GroupNotFoundException(key);
        
        return GroupResponse.FromEntity(group);
    }

    public async Task DeleteAsync(string key)
    {
        var group = await _authDbContext.Groups.FirstOrDefaultAsync(s => s.Key == key);

        if (group is null)
            throw new GroupNotFoundException(key);

        _authDbContext.Groups.Remove(group);
        await _authDbContext.SaveChangesAsync();
    }

    public async Task<GroupResponse> UpdateAsync(string key, UpdateGroupRequest request)
    {
        var group = await _authDbContext.Groups
            .Include(s => s.GroupRoles)
            .ThenInclude(s => s.Role)
            .FirstOrDefaultAsync(s => s.Key == key);
        
        if (group is null)
            throw new GroupNotFoundException(key);
        
        var requestRoles = await _authDbContext.Roles.Where(s => request.Roles.Contains(s.Key)).ToListAsync();
        var foundRoleKeys = requestRoles.Select(r => r.Key).ToHashSet();
        var missingRoles = request.Roles.Except(foundRoleKeys).ToList();
        
        if (missingRoles.Count != 0)
            throw new RoleNotFoundException(missingRoles);
        
        var removedRoles = group.GroupRoles.Where(s => !request.Roles.Contains(s.Role.Key)).Select(s => s.Role).ToList();
        var addedRoles = requestRoles.Where(s => !group.GroupRoles.Select(gr => gr.Role.Key).Contains(s.Key)).ToList();
        
        foreach (var role in removedRoles)
            group.RemoveRole(role);
        
        foreach (var role in addedRoles)
            group.AddRole(role);
        
        group.Update(request.Key, request.Name, request.Description);
        
        await _authDbContext.SaveChangesAsync();
        
        return GroupResponse.FromEntity(group);
    }
    
    private void EnsureMissingRoles(List<string> requestRoles, List<Role> roles)
    {
        var foundRoleKeys = roles.Select(r => r.Key).ToHashSet();
        var missingRoles = requestRoles.Except(foundRoleKeys).ToList();

        if (missingRoles.Count != 0)
            throw new RoleNotFoundException(missingRoles);
    }
}