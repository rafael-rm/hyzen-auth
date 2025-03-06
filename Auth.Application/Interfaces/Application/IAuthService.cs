using Auth.Application.Common;

namespace Auth.Application.Interfaces.Application;

public interface IAuthService
{
    Task<Result> LoginAsync(string email, string password);
    Task<Result> VerifyAsync(string token);
}