using Auth.Domain.Entities;

namespace Auth.Application.Interfaces.Infrastructure;

public interface ITokenService
{
    string GenerateToken(User user);
    Task<bool> VerifyAsync(string token);
}