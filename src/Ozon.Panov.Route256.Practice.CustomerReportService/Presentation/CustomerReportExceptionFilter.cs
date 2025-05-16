using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ozon.Panov.Route256.Practice.CustomerReportService.Domain;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Presentation;

internal sealed class CustomerReportExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is ExternalServiceInvalidArgumentException 
            || context.Exception is RpcException rpcException
            && rpcException.StatusCode == StatusCode.Cancelled)
        {
            context.Result = new BadRequestObjectResult(context.Exception.Message);
            context.ExceptionHandled = true;
        }
        else if (context.Exception is OperationCanceledException)
        {
            context.Result = new ObjectResult("Request canceled.")
            {
                StatusCode = StatusCodes.Status499ClientClosedRequest
            };
            context.ExceptionHandled = true;
        }
        else if (context.Exception is RpcException rpcError
            && rpcError.StatusCode == StatusCode.Unavailable)
        {
            context.Result = new ObjectResult("Request timeout.")
            {
                StatusCode = StatusCodes.Status408RequestTimeout
            };
            context.ExceptionHandled = true;
        }
    }
}

