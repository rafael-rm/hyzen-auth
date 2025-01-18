using Hyzen.SDK.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Legacy.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomActionFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        HyzenAuth.SetToken(context.HttpContext.Request.Headers.Authorization);
        
        var route = context.HttpContext.Request.Path.Value;

        if (NeedsAuthentication(route, context.HttpContext.Request.Method))
        {
            // Ensures that the user is authenticated and pre-loads the subject
            var subject = await HyzenAuth.GetSubject();
            HyzenAuth.SetSubject(subject);
        }
        
        await next();
    }
    
    private bool NeedsAuthentication(string route, string method)
    {
        var noAuthentication = new List<(string route, string method)>
        {
            new("/api/v1/Auth/Login", "POST"),
            new("/api/v1/Auth/Verify", "POST"),
            new("/api/v1/Auth/SendRecoveryEmail", "POST"),
            new("/api/v1/Auth/RecoverPassword", "POST"),
            new("/api/v1/User", "POST")
        };
    
        return !noAuthentication.Any(s => route is not null && method is not null && route.StartsWith(s.route, StringComparison.OrdinalIgnoreCase) && method.Equals(s.method, StringComparison.OrdinalIgnoreCase));
    }

}