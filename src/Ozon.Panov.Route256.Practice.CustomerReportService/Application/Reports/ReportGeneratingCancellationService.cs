using System.Collections.Concurrent;

namespace Ozon.Panov.Route256.Practice.CustomerReportService.Application.Reports;

public class ReportGeneratingCancellationService
{
    private readonly ConcurrentDictionary<long, CancellationTokenSource> _cancellationTokens = new();

    public CancellationToken RegisterRequestForCustomer(long customerId, CancellationToken cancellationToken)
    {
        var tokenSource = _cancellationTokens
            .AddOrUpdate(
                customerId,
                _ => CancellationTokenSource.CreateLinkedTokenSource(cancellationToken),
                (_, oldToken) =>
                {
                    oldToken.Cancel();
                    oldToken.Dispose();
                    return CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                });

        return tokenSource.Token;
    }

    public void FinishRequestProcessingForCustomer(long customerId)
    {
        if (_cancellationTokens.TryRemove(customerId, out var tokenSource))
        {
            tokenSource.Dispose();
        }
    }
}