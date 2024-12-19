using System.Net;
using Hyzen.SDK.Exception;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth.Core.Filters;

public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        SentrySdk.CaptureException(context.Exception);
        
        context.Exception = HException.FromException(context.Exception);
        
        context.Result = new ObjectResult(((HException)context.Exception).ToErrorObject())
        {
            StatusCode = (int)((HException)context.Exception).StatusCode
        };
        
        context.Exception = null!;
    }
}