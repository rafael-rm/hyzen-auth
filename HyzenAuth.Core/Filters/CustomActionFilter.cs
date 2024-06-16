using Hyzen.SDK.Authentication;
using HyzenAuth.Core.Helper;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HyzenAuth.Core.Filters;

public class CustomActionFilter : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var token = TokenHelper.GetToken(context.HttpContext);
        Auth.SetToken(token);
        
        await next(); // Execute the next action
    }
}