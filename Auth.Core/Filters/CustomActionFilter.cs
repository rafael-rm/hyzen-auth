using Hyzen.SDK.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth.Core.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomActionFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        HyzenAuth.SetToken(context.HttpContext.Request.Headers.Authorization);
        
        var route = context.HttpContext.Request.Path.Value;

        var noAuthentication = new List<string> { "/api/v1/Auth", "/api/v1/Login", };

        if (!noAuthentication.Any(s => route is not null && route.StartsWith(s)))
        {
            await HyzenAuth.EnsureAuthenticated();
        }
        
        await next();
    }
}