using Auth.Domain.Entities;

namespace Auth.Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(User user);
    Task<bool> VerifyAsync(string token);
}