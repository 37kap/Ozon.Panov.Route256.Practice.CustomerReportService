using CsvHelper;
using System.Globalization;
using System.Text;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Domain.CsvOrdersReport;

internal static class CsvOrdersReportSerializer
{
    public static async Task<Stream> Serialize(
        IEnumerable<OrderReportRecord> records,
        CancellationToken cancellationToken)
    {
        var memoryStream = new MemoryStream();

        await using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8, bufferSize: 4096, leaveOpen: true))
        await using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture, leaveOpen: true))
        {
            await csvWriter.WriteRecordsAsync(records, cancellationToken);
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }
}
