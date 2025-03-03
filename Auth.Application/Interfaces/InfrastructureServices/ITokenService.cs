using Auth.Domain.Entities;

namespace Auth.Application.Interfaces.InfrastructureServices;

public interface ITokenService
{
    string GenerateToken(User user);
    Task<bool> VerifyAsync(string token);
}