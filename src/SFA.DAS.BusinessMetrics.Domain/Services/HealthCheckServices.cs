using Azure.Core;
using Azure.Monitor.Query;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Domain.Services
{
    public record HealthCheckServices : IHealthCheckServices
    {
        private readonly ILogsQueryClient _queryClient;
        private readonly LogAnalyticsWorkSpace _logAnalyticsWorkSpaceConfiguration;

        public HealthCheckServices(
            IOptions<LogAnalyticsWorkSpace> logWorkspaceConfigurationOptions,
            ILogsQueryClient queryClient)
        {
            _queryClient = queryClient;
            _logAnalyticsWorkSpaceConfiguration = logWorkspaceConfigurationOptions.Value;
        }

        public async Task<HealthCheckResult> GetHealthCheck(CancellationToken token)
        {
            var result = await _queryClient.ProcessQuery(
                new ResourceIdentifier(_logAnalyticsWorkSpaceConfiguration.Identifier),
                $"Heartbeat " +
                $"| summarize count() by Computer " +
                $"| project Computer",
                new QueryTimeRange(DateTimeOffset.UtcNow.AddMinutes(-30), DateTimeOffset.UtcNow),
                token);

            return result is { Rows.Count: > 0 }
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Degraded();
        }
    }
}