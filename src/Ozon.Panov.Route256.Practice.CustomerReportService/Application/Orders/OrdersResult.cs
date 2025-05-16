using Ozon.Panov.Route256.Practice.CustomerReportService.Domain.Orders;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Application.Orders;

public sealed record OrdersResult(
    IReadOnlyCollection<Order> Orders,
    long TotalCount);
