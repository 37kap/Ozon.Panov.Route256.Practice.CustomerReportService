using Ozon.Panov.Route256.Practice.CustomerReportService.Application.Orders;
using Ozon.Panov.Route256.Practice.CustomerReportService.Domain.CsvOrdersReport;
using System.Threading.Tasks.Dataflow;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Application.Reports;

internal sealed class OrdersReportBatchProcessor(
    IOrdersProvider ordersProvider) : IOrdersReportBatchProcessor
{
    public async Task<IReadOnlyCollection<OrderReportRecord>> GetReportRecords(
        long customerId,
        CancellationToken cancellationToken)
    {
        const int batchSize = 100;
        var ordersCount = await CountOrdersAsync(customerId, cancellationToken);
        var numberOfBatches = (int)Math.Ceiling((double)ordersCount / batchSize);
        var tasks = new List<Task<IReadOnlyCollection<OrderReportRecord>>>();

        for (int batchNumber = 0; batchNumber < numberOfBatches; batchNumber++)
        {
            tasks.Add(PrepareOrdersTaskAsync(customerId, batchSize, batchNumber, cancellationToken));
        }

        var results = await Task.WhenAll(tasks);
        var orders = new List<OrderReportRecord>(capacity: (int)ordersCount);

        foreach (var batch in results)
        {
            orders.AddRange(batch);
        }

        return orders;
    }

    private async Task<IReadOnlyCollection<OrderReportRecord>> PrepareOrdersTaskAsync(
        long customerId,
        int batchSize,
        int batchNumber,
        CancellationToken cancellationToken)
    {
        var offset = batchNumber * batchSize;

        var ordersResponse = await ordersProvider.GetOrders(
            new OrdersQuery(
                Limit: batchSize,
                Offset: (int)offset,
                CustomerId: customerId),
            cancellationToken);

        var reportRecords = ordersResponse.Orders.Select(order =>
            order.ToReportRecord()).ToArray();

        return reportRecords;
    }

    private async Task<long> CountOrdersAsync(long customerId, CancellationToken cancellationToken)
    {
        OrdersResult ordersResult = await ordersProvider
            .GetOrders(
                new OrdersQuery(
                    Limit: 1,
                    Offset: 0,
                    CustomerId: customerId),
                cancellationToken);

        return ordersResult.TotalCount;
    }
}