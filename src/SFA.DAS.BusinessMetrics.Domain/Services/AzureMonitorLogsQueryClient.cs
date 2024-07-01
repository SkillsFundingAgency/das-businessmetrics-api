using Azure.Core;
using Azure.Identity;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Domain.Services
{
    public class AzureMonitorLogsQueryClient : ILogsQueryClient
    {
        public async Task<LogsTable> ProcessQuery(
            ResourceIdentifier resourceIdentifier,
            string query,
            QueryTimeRange timeRange,
            CancellationToken token)
        {
            var logsClient = new LogsQueryClient(new AzureCliCredential());

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
