using Azure.Core;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Domain.Services
{
    public class AzureMonitorLogsQueryClient(LogsQueryClient client) : ILogsQueryClient
    {
        public async Task<LogsTable> ProcessQuery(
            ResourceIdentifier resourceIdentifier,
            string query,
            QueryTimeRange timeRange,
            CancellationToken token)
        {
            var result = await client.QueryResourceAsync(
                resourceIdentifier,
                query,
                timeRange,
                cancellationToken: token);

            return result.Value.Table;
        }
    }
}