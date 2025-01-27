using Auth.Domain.Entities;

namespace Auth.Application.Services;

public interface ITokenService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
}