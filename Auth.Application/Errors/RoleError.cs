using Auth.Application.Common;

namespace Auth.Application.Errors;

public static class RoleError
{
    public static ResultError RoleAlreadyExists => new(ErrorTypeConstant.Conflict, "Role already exists.");
    public static ResultError RoleNotFound => new(ErrorTypeConstant.NotFound, "Role not found.");
}