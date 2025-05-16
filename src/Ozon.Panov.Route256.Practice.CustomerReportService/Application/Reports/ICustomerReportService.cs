namespace Ozon.Panov.Route256.Practice.CustomerReportService.Application.Reports;

public interface ICustomerReportService
{
    Task<Stream> GenerateReport(long customerId, CancellationToken cancellationToken);
}