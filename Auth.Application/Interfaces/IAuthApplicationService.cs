using Auth.Application.DTOs.Response;

namespace Auth.Application.Interfaces;

public interface IAuthApplicationService
{
    Task<LoginResponse> LoginAsync(string email, string password);
    Task VerifyAsync(string token);
}