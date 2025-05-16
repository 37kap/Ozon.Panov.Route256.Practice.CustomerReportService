using Google.Api;
using Ozon.Panov.Route256.Practice.CustomerReportService.Domain.CsvOrdersReport;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Application.Reports;

internal sealed class CustomerReportService(
    IOrdersReportBatchProcessor batchProcessor) : ICustomerReportService
{
    public async Task<Stream> GenerateReport(long customerId, CancellationToken cancellationToken)
    {
        var records = await batchProcessor.GetReportRecords(customerId, cancellationToken);
        var stream = await CsvOrdersReportSerializer.Serialize(records, cancellationToken);

        return stream;
    }
}
