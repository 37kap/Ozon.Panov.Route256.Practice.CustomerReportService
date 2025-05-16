namespace Ozon.Panov.Route256.Practice.CustomerReportService.Application.Orders;

public sealed record OrdersQuery(
    long CustomerId,
    int Limit,
    int Offset);