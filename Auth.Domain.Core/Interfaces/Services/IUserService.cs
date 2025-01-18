using Auth.Domain.Entities;

namespace Auth.Domain.Core.Interfaces.Services;

public interface IUserService
{
    Task CreateAsync(User user);
    Task<User?> GetByGuidAsync(Guid guid);
    Task<User?> GetByEmailAsync(string email);
}