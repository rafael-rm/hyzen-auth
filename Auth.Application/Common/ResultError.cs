namespace Auth.Application.Common;

public sealed record ResultError(string Code, string Message)
{
    internal static ResultError None => new(ErrorTypeConstant.None, string.Empty);
}