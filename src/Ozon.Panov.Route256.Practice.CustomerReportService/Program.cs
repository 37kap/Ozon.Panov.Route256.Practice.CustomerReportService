namespace Ozon.Panov.Route256.Practice.CustomerReportService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            IConfigurationRoot configuration = builder.Configuration
                .AddEnvironmentVariables("ROUTE256_")
                .Build();

            builder.Services
                .AddApplication(configuration)
                .AddInfrastructure(configuration)
                .AddPresentation();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseRouting();
            app.MapControllers();

            app.Run();
        }
    }
}
