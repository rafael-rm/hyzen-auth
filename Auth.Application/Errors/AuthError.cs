using Auth.Application.Common;

namespace Auth.Application.Errors;

public static class AuthError
{
    public static ResultError InvalidToken => new(ErrorTypeConstant.Unauthorized, "Invalid or expired token.");
    public static ResultError InvalidCredentials => new(ErrorTypeConstant.Unauthorized, "Invalid credentials.");
}