using Auth.Application.DTOs.Response;

namespace Auth.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(string email, string password);
    Task VerifyAsync(string token);
}