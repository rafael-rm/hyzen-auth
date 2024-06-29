using Hyzen.SDK.Authentication;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HyzenAuth.Core.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CustomActionFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        Auth.SetToken(context.HttpContext.Request.Headers.Authorization);
        
        await next(); // Execute the next action
    }
}