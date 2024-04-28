using System.Net;
using System.Security.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HyzenAuth.Core.Filters;

public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
{
    private readonly Dictionary<Type, HttpStatusCode> _exceptionMappings = new()
    {
        { typeof(NotImplementedException), HttpStatusCode.NotImplemented },
        { typeof(ArgumentException), HttpStatusCode.BadRequest },
        { typeof(InvalidOperationException), HttpStatusCode.BadRequest },
        { typeof(TimeoutException), HttpStatusCode.RequestTimeout },
        { typeof(AuthenticationException), HttpStatusCode.Unauthorized },
        { typeof(UnauthorizedAccessException), HttpStatusCode.Forbidden }
    };

    public override void OnException(ExceptionContext context)
    {
        if (_exceptionMappings.TryGetValue(context.Exception.GetType(), out var statusCode))
        {
            context.Result = new ObjectResult(new { error = context.Exception.Message })
            {
                StatusCode = (int)statusCode
            };
        }
        else
        {
            SentrySdk.CaptureException(context.Exception);
            
            context.Result = new ObjectResult(new { error = "An unexpected error occurred" })
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }

        context.Exception = null!;
    }
}