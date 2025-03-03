using Auth.Application.Common;

namespace Auth.Application.Errors;

public static class UserError
{
    public static ResultError UserNotFound => new(ErrorTypeConstant.NotFound, "User not found.");
    public static ResultError UserAlreadyExists => new(ErrorTypeConstant.Conflict, "User already exists.");
}