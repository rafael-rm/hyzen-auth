using Auth.Domain.Entities;

namespace Auth.Domain.Interfaces.Services;

public interface IGroupService
{
    Task<Group> CreateAsync(Group role);
    Task<Group?> GetByGuidAsync(Guid groupId);
    Task<Group?> GetByNameAsync(string name);
    Task DeleteAsync(Group role);
}