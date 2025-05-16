namespace Ozon.Panov.Route256.Practice.CustomerReportService.Domain.Orders;

public sealed record Order(
    long OrderId,
    OrderStatus Status,
    long CustomerId,
    string? Comment,
    DateTime CreatedAt);