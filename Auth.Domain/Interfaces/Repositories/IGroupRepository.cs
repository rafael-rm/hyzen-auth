using Auth.Domain.Entities;

namespace Auth.Domain.Interfaces.Repositories;

public interface IGroupRepository
{
    Task<Group?> GetByIdAsync(int id);
    Task<Group?> GetByGuidAsync(Guid groupId);
    Task<Group?> GetByNameAsync(string name);
    Task<IEnumerable<Group>> GetAllAsync();
    Task AddAsync(Group role);
    void Delete(Group role);
}