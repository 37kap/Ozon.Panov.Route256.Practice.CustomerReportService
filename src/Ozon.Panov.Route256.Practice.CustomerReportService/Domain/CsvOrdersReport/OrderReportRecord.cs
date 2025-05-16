using CsvHelper.Configuration.Attributes;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Domain.CsvOrdersReport;

public sealed class OrderReportRecord
{
    [Name("Id заказа")]
    public long OrderId { get; set; }

    [Name("Статус")]
    public string OrderStatus { get; set; } = String.Empty;

    [Name("Коммент")]
    public string Comment { get; set; } = String.Empty;

    [Name("Дата создания")]
    public DateTime CreatedAt { get; set; }
}