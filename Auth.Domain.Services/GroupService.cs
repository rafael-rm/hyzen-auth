using Auth.Domain.Entities;
using Auth.Domain.Exceptions.Role;
using Auth.Domain.Interfaces.Repositories;
using Auth.Domain.Interfaces.Services;

namespace Auth.Domain.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    
    public GroupService(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }
    
    public async Task<Group> CreateAsync(Group role)
    {
        var existingRole = await _groupRepository.GetByNameAsync(role.Name);
        
        if (existingRole != null)
            throw new RoleAlreadyExistsException(existingRole.Name);
        
        await _groupRepository.AddAsync(role);
        
        return role;
    }

    public async Task<Group?> GetByGuidAsync(Guid groupId)
    {
        return await _groupRepository.GetByGuidAsync(groupId);
    }

    public async Task<Group?> GetByNameAsync(string name)
    {
        return await _groupRepository.GetByNameAsync(name);
    }

    public Task DeleteAsync(Group role)
    {
        _groupRepository.Delete(role);
        
        return Task.CompletedTask;
    }
}