using Microsoft.Extensions.Options;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Application.RateLimit;

internal sealed class RateLimiter(IOptions<RateLimitSettings> rateLimitSettings) : IDisposable
{
    public SemaphoreSlim Semaphore { get; } = new(
        rateLimitSettings.Value.ReportServiceRateLimit,
        rateLimitSettings.Value.ReportServiceRateLimit);

    public void Dispose()
    {
        Semaphore.Dispose();
    }
}