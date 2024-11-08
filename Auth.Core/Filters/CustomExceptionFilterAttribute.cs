using System.Net;
using Hyzen.SDK.Exception;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Auth.Core.Filters;

public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        context.Exception = HException.FromException(context.Exception);

        if (((HException)context.Exception).Type != ExceptionType.InternalError)
        {
            context.Result = new ObjectResult(new { error = context.Exception.Message })
            {
                StatusCode = (int)((HException)context.Exception).StatusCode
            };
        }
        else
        {
            context.Result = new ObjectResult(new { error = "An unexpected error occurred while processing your request" })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
        
        SentrySdk.CaptureException(context.Exception);
        
        context.Exception = null!;
    }
}