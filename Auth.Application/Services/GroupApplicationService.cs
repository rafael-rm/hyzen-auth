using Auth.Application.DTOs.Request;
using Auth.Application.DTOs.Response;
using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Domain.Exceptions.Group;
using Auth.Domain.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace Auth.Application.Services;

public class GroupApplicationService : IGroupApplicationService
{
    private readonly IAuthDbContext _authDbContext;
    private readonly IGroupService _groupService;
    
    public GroupApplicationService(IAuthDbContext authDbContext, IGroupService groupService)
    {
        _authDbContext = authDbContext;
        _groupService = groupService;
    }
    
    public async Task<GroupResponse> CreateAsync(CreateGroupRequest createGroupRequest)
    {
        var group = new Group(createGroupRequest.Name, createGroupRequest.Description);
        
        await _authDbContext.Groups.AddAsync(group);
        await _authDbContext.SaveChangesAsync();
        
        return GroupResponse.FromEntity(group);
    }

    public async Task<GroupResponse> GetByGuidAsync(Guid groupId)
    {
        var group = await _authDbContext.Groups.FirstOrDefaultAsync(s => s.Guid == groupId);

        if (group is null)
            throw new GroupNotFoundException(groupId);
        
        return GroupResponse.FromEntity(group);
    }

    public async Task<GroupResponse> GetByNameAsync(string name)
    {
        var group = await _authDbContext.Groups.FirstOrDefaultAsync(s => s.Name == name);
        
        if (group is null)
            throw new GroupNotFoundException(name);
        
        return GroupResponse.FromEntity(group);
    }

    public async Task DeleteAsync(string name)
    {
        var group = await _authDbContext.Groups.FirstOrDefaultAsync(s => s.Name == name);

        if (group is null)
            throw new GroupNotFoundException(name);

        _authDbContext.Groups.Remove(group);
        await _authDbContext.SaveChangesAsync();
    }
}