using Auth.Domain.Entities;

namespace Auth.Domain.Core.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(User user);
    Task<bool> VerifyAsync(string token);
}