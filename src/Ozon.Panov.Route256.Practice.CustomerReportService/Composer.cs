using Grpc.Core;
using Grpc.Net.Client.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Ozon.Panov.Route256.Practice.CustomerReportService.Application.Orders;
using Ozon.Panov.Route256.Practice.CustomerReportService.Application.RateLimit;
using Ozon.Panov.Route256.Practice.CustomerReportService.Application.Reports;
using Ozon.Panov.Route256.Practice.CustomerReportService.Infrastructure.Orders;
using Ozon.Panov.Route256.Practice.CustomerReportService.Presentation;
using Ozon.Route256.OrderService.Proto.OrderGrpc;
using System.Reflection;
using ReportService = Ozon.Panov.Route256.Practice.CustomerReportService.Application.Reports.CustomerReportService;

namespace Ozon.Panov.Route256.Practice.CustomerReportService;

internal static class Composer
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddScoped<IOrdersReportBatchProcessor, OrdersReportBatchProcessor>()
            .AddTransient<ICustomerReportService, ReportService>()
            .AddReportGeneratingCancellation()
            .AddSimpleRateLimiter(configuration);
    }

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOrderServiceGrpcClient(configuration);
        return services
            .AddSingleton<OrderServiceLimiter>()
            .AddScoped<IOrdersProvider, OrdersProvider>();
    }

    private static IServiceCollection AddSimpleRateLimiter(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<RateLimitSettings>()
            .Bind(configuration.GetSection(nameof(RateLimitSettings)))
            .Validate(settings =>
            {
                return settings.ReportServiceRateLimit > 0 && settings.OrderServiceRateLimit > 0;
            }, 
            "ReportServiceRateLimit and OrderServiceRateLimit must be greater than 0.");


        services.AddSingleton<RateLimiter>();
        services.AddSingleton<RateLimitFilter>();

        return services;
    }

    private static IServiceCollection AddReportGeneratingCancellation(
        this IServiceCollection services)
    {
        services
            .AddSingleton<ReportGeneratingCancellationService>();

        return services;
    }

    private static IServiceCollection AddOrderServiceGrpcClient(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var orderServiceUrl = configuration.GetValue<string>("ORDER_SERVICE_URL");

        services.AddGrpcClient<OrderGrpc.OrderGrpcClient>(options =>
        {
            options.Address = new Uri(orderServiceUrl!);
        })
            .ConfigureChannel(options =>
            {
                options.ServiceConfig = new ServiceConfig
                {
                    MethodConfigs = { GetDefaultMethodConfig() }
                };
            });

        return services;
    }

    private static MethodConfig GetDefaultMethodConfig()
    {
        return new MethodConfig
        {
            Names = { MethodName.Default },
            RetryPolicy = new RetryPolicy
            {
                MaxAttempts = 5,
                InitialBackoff = TimeSpan.FromMilliseconds(10),
                MaxBackoff = TimeSpan.FromMilliseconds(25),
                BackoffMultiplier = 1.5,
                RetryableStatusCodes = { StatusCode.Unavailable }
            }
        };
    }
}
