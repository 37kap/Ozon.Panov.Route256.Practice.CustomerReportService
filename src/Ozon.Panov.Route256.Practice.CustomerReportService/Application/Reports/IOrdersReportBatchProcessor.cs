using Ozon.Panov.Route256.Practice.CustomerReportService.Domain.CsvOrdersReport;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Application.Reports;

public interface IOrdersReportBatchProcessor
{
    Task<IReadOnlyCollection<OrderReportRecord>> GetReportRecords(
        long customerId,
        CancellationToken cancellationToken);
}
