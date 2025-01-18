using Auth.Domain.Entities;

namespace Auth.Domain.Core.Interfaces.Services;

public interface IUserService
{
    Task AddAsync(User user);
}