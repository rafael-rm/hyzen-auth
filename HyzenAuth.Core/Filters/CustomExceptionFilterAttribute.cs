using System.Net;
using Hyzen.Util.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HyzenAuth.Core.Filters;

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
            context.Result = new ObjectResult(new { error = "An unexpected error occurred" })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
        
        SentrySdk.CaptureException(context.Exception);
        
        context.Exception = null!;
    }
}