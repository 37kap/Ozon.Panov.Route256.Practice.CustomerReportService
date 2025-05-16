using Ozon.Panov.Route256.Practice.CustomerReportService.Domain.CsvOrdersReport;
using Ozon.Panov.Route256.Practice.CustomerReportService.Domain.Orders;
using GrpcOrderStatus = Ozon.Route256.OrderService.Proto.OrderGrpc.OrderStatus;
using V1QueryOrdersResponse = Ozon.Route256.OrderService.Proto.OrderGrpc.V1QueryOrdersResponse;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Application.Orders;

internal static class OrderMapper
{
    public static Order FromGrpcResponse(this V1QueryOrdersResponse ordersResponse)
        => new(
            OrderId: ordersResponse.OrderId,
            Status: ordersResponse.Status.FromGrpcResponse(),
            CustomerId: ordersResponse.CustomerId,
            Comment: ordersResponse.Comment,
            CreatedAt: ordersResponse.CreatedAt.ToDateTime());

    public static OrderStatus FromGrpcResponse(this GrpcOrderStatus grpcStatus)
        => grpcStatus switch
        {
            GrpcOrderStatus.Undefined => OrderStatus.Undefined,
            GrpcOrderStatus.New => OrderStatus.New,
            GrpcOrderStatus.Canceled => OrderStatus.Canceled,
            GrpcOrderStatus.Delivered => OrderStatus.Delivered,
            _ => throw new ArgumentOutOfRangeException(nameof(grpcStatus), grpcStatus, null)
        };

    public static OrderReportRecord ToReportRecord(this Order order)
        => new()
        {
            OrderId = order.OrderId,
            OrderStatus = order.Status.ToString("G"),
            Comment = order.Comment ?? string.Empty,
            CreatedAt = order.CreatedAt
        };
}