using Microsoft.Extensions.Options;
using Ozon.Panov.Route256.Practice.CustomerReportService.Application.RateLimit;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Infrastructure.Orders;

internal sealed class OrderServiceLimiter(IOptions<RateLimitSettings> rateLimitSettings) : IDisposable
{
    public SemaphoreSlim Semaphore { get; } = new(
        rateLimitSettings.Value.OrderServiceRateLimit,
        rateLimitSettings.Value.OrderServiceRateLimit);

    public void Dispose()
    {
        Semaphore.Dispose();
    }
}