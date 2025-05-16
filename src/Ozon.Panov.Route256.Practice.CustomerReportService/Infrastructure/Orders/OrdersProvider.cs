using Grpc.Core;
using Ozon.Panov.Route256.Practice.CustomerReportService.Application.Orders;
using Ozon.Panov.Route256.Practice.CustomerReportService.Domain;
using Ozon.Panov.Route256.Practice.CustomerReportService.Domain.Orders;
using Ozon.Route256.OrderService.Proto.OrderGrpc;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Infrastructure.Orders;

internal sealed class OrdersProvider(
    OrderGrpc.OrderGrpcClient orderGrpcClient,
    OrderServiceLimiter orderServiceLimiter)
    : IOrdersProvider
{
    public async Task<OrdersResult> GetOrders(
        OrdersQuery query,
        CancellationToken cancellationToken)
    {
        var request = new V1QueryOrdersRequest
        {
            Limit = query.Limit,
            Offset = query.Offset
        };

        if (query.CustomerId is { } customerId)
        {
            request.CustomerIds.Add(customerId);
        }

        List<Order> orders = [];
        long? totalCount = null;

        await orderServiceLimiter.Semaphore.WaitAsync(cancellationToken);

        try
        {
            using var ordersStream = orderGrpcClient
                .V1QueryOrders(
                request,
                cancellationToken: cancellationToken);

            while (await ordersStream.ResponseStream.MoveNext())
            {
                var ordersResponse = ordersStream.ResponseStream.Current;
                totalCount ??= ordersResponse.TotalCount;
                orders.Add(ordersResponse.FromGrpcResponse());
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.InvalidArgument)
        {
            throw new ExternalServiceInvalidArgumentException(ex.Message, ex);
        }
        finally
        {
            orderServiceLimiter.Semaphore.Release();
        }

        var ordersResult = new OrdersResult(
            Orders: orders,
            TotalCount: totalCount ?? 0);

        return ordersResult;
    }
}
