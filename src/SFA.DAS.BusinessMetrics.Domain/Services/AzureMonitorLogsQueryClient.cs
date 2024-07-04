using Azure.Core;
using Azure.Identity;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Domain.Services
{
    public class AzureMonitorLogsQueryClient : ILogsQueryClient
    {
        private const int MaxRetries = 2;
        private readonly TimeSpan _networkTimeout = TimeSpan.FromSeconds(1);
        private readonly TimeSpan _delay = TimeSpan.FromMilliseconds(100);
        public async Task<LogsTable> ProcessQuery(
            ResourceIdentifier resourceIdentifier,
            string query,
            QueryTimeRange timeRange,
            CancellationToken token)
        {
            var logsClient = new LogsQueryClient(new ChainedTokenCredential(
                new ManagedIdentityCredential(options: new TokenCredentialOptions
                {
                    Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay }
                }),
                new AzureCliCredential(options: new AzureCliCredentialOptions
                {
                    Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay }
                }),
                new VisualStudioCredential(options: new VisualStudioCredentialOptions
                {
                    Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay }
                }),
                new VisualStudioCodeCredential(options: new VisualStudioCodeCredentialOptions()
                {
                    Retry = { NetworkTimeout = _networkTimeout, MaxRetries = MaxRetries, Delay = _delay }
                })));

            var logsResults = await logsClient.QueryResourceAsync(
                resourceIdentifier,
                query,
                timeRange,
                null,
                token);

            return logsResults.Value.Table;
        }
    }
}
