using Microsoft.AspNetCore.Mvc;
using Ozon.Panov.Route256.Practice.CustomerReportService.Application.Reports;
using Swashbuckle.AspNetCore.Annotations;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Presentation;

[ApiController]
[Route("report")]
[CustomerReportExceptionFilter]
[ServiceFilter<RateLimitFilter>]
public class CustomerReportController(
    ICustomerReportService reportService,
    ReportGeneratingCancellationService cancellationService) : ControllerBase
{
    /// <summary>
    /// Сформировать отчет с историей заказов пользователя в формате CSV.
    /// </summary>
    /// <param name="request">Параметры для генерации отчета.</param>
    /// <param name="cancellationToken">Токен отмены генерации отчета.</param>
    /// <returns>Отчет в формате CSV с заказами пользователя.</returns>
    /// <response code="200">Успешная обработка запроса.</response>
    /// <response code="400">Некорректные параметры запроса.</response>
    /// <response code="499">Отмена запроса по токену.</response>
    /// <response code="500">Произошла внутренняя ошибка сервера.</response>
    [HttpGet("generate")]
    [SwaggerOperation(Summary = "Сформировать отчет с историей заказов пользователя в формате CSV.")]
    [SwaggerResponse(StatusCodes.Status200OK, "Отчет в формате CSV с заказами пользователя.", typeof(FileStreamResult))]
    public async Task<IActionResult> Get([FromQuery] GenerateReportRequest request, CancellationToken cancellationToken)
    {
        if (request.CustomerId <= 0)
        {
            return BadRequest("CustomerId must be greater than 0.");
        }

        cancellationToken = cancellationService.RegisterRequestForCustomer(
            request.CustomerId,
            cancellationToken);

        var stream = await reportService.GenerateReport(request.CustomerId, cancellationToken);

        cancellationService
            .FinishRequestProcessingForCustomer(request.CustomerId);

        return File(
            stream,
            contentType: "text/csv",
            fileDownloadName: $"customer_{request.CustomerId}_report_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
    }
}
