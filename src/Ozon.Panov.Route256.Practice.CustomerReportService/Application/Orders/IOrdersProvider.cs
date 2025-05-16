namespace Ozon.Panov.Route256.Practice.CustomerReportService.Application.Orders;

public interface IOrdersProvider
{
    Task<OrdersResult> GetOrders(
        OrdersQuery query,
        CancellationToken cancellationToken);
}
