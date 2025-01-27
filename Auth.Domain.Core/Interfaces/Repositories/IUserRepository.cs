using Auth.Domain.Entities;

namespace Auth.Domain.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByGuidAsync(Guid userId);
    Task<User?> GetByEmailAsync(string email);
    Task UpdateAsync(User user);
    void Delete(User user);
}