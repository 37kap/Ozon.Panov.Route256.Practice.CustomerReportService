using Microsoft.AspNetCore.Mvc.Filters;
using Ozon.Panov.Route256.Practice.CustomerReportService.Application.RateLimit;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Presentation;

internal sealed class RateLimitFilter(RateLimiter rateLimiter) : IAsyncActionFilter, IDisposable
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var cancellationToken = context.HttpContext.RequestAborted;

        await rateLimiter.Semaphore.WaitAsync(cancellationToken);

        try
        {
            await next();
        }
        finally
        {
            rateLimiter.Semaphore.Release();
        }
    }

    public void Dispose()
    {
        rateLimiter.Dispose();
    }
}