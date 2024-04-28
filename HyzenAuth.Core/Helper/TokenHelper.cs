namespace HyzenAuth.Core.Helper;

public static class TokenHelper
{
    public static string GetToken(HttpContext httpContext)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        var authorizationHeader = httpContext.Request.Headers.Authorization.FirstOrDefault();
        
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return null;
        
        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

        return token;
    }
}