namespace Ozon.Panov.Route256.Practice.CustomerReportService.Domain;

internal sealed class ExternalServiceInvalidArgumentException(
    string message, Exception? innerException = null)
    : Exception(message, innerException);