using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using Ozon.Panov.Route256.Practice.CustomerReportService.Application.RateLimit;
using Ozon.Panov.Route256.Practice.CustomerReportService.Application.Reports;
using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.IntegrationTests;

public class CustomerReportControllerTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;
    private Mock<ICustomerReportService> _mockReportService;

    public Task InitializeAsync()
    {
        _mockReportService = new Mock<ICustomerReportService>();

        _mockReportService
            .Setup(service => 
                service.GenerateReport(
                    It.IsAny<long>(), 
                    It.IsAny<CancellationToken>()))
            .Returns(async (long _, CancellationToken cancellationToken) =>
                {
                    await Task.Delay(1000, cancellationToken);
                    return new MemoryStream();
                });

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.Configure<RateLimitSettings>(options =>
                    {
                        options.ReportServiceRateLimit = 2;
                    });

                    services.AddSingleton(_mockReportService.Object);
                });
            });

        _client = _factory.CreateDefaultClient();

        return Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
    }

    [Fact]
    public async Task Enforces_the_rate_limit_when_generating_a_report()
    {
        var tasks = new List<Task<HttpResponseMessage>>
        {
            _client.GetAsync("/report/generate?customerId=1"),
            _client.GetAsync("/report/generate?customerId=2"),
            _client.GetAsync("/report/generate?customerId=3")
        };

        var responses = await Task.WhenAll(tasks.Take(2));

        Assert.All(responses, r => Assert.Equal(HttpStatusCode.OK, r.StatusCode));

        var delayedResponse = await tasks.Last();

        Assert.Equal(HttpStatusCode.OK, delayedResponse.StatusCode);

        _mockReportService.Verify(m => 
            m.GenerateReport(It.IsAny<long>(), It.IsAny<CancellationToken>()), 
            Times.Exactly(3));
    }

    [Fact]
    public async Task Should_cancel_previous_request_for_same_customer()
    {
        var firstRequestTask = _client.GetAsync("/report/generate?customerId=1");
        await Task.Delay(100);
        var secondRequestTask = _client.GetAsync("/report/generate?customerId=1");
        await Task.Delay(100);
        var anotherCustomerRequestTask = _client.GetAsync("/report/generate?customerId=2");

        var secondResponse = await secondRequestTask;
        var firstResponse = await firstRequestTask;
        var anotherResponse = await anotherCustomerRequestTask;

        Assert.False(firstResponse.IsSuccessStatusCode);
        Assert.True(secondResponse.IsSuccessStatusCode);
        Assert.True(anotherResponse.IsSuccessStatusCode);

        _mockReportService.Verify(m => 
            m.GenerateReport(It.IsAny<long>(), It.IsAny<CancellationToken>()), 
            Times.Exactly(3));
    }
}