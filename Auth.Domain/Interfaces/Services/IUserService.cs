using Auth.Domain.Entities;

namespace Auth.Domain.Interfaces.Services;

public interface IUserService
{
    Task<User> CreateAsync(User user);
    Task<User?> GetByGuidAsync(Guid userId);
    Task<User?> GetByEmailAsync(string email);
    Task DeleteAsync(User user);
}