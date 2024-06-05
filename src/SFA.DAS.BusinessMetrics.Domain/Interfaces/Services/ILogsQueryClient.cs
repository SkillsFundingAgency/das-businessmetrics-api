using Azure.Core;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;

namespace SFA.DAS.BusinessMetrics.Domain.Interfaces.Services
{
    public interface ILogsQueryClient
    {
        Task<LogsTable> ProcessQuery(ResourceIdentifier resourceIdentifier, string query, QueryTimeRange timeRange, CancellationToken token);
    }
}
