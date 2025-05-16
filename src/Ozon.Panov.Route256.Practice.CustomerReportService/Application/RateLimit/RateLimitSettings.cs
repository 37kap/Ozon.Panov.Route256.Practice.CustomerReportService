namespace Ozon.Panov.Route256.Practice.CustomerReportService.Application.RateLimit;

public sealed class RateLimitSettings
{
    public int ReportServiceRateLimit { get; set; }
    public int OrderServiceRateLimit { get; set; }
}