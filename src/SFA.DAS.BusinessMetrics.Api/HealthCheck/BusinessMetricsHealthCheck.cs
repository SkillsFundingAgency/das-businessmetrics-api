using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SFA.DAS.BusinessMetrics.Api.HealthCheck
{
    public class BusinessMetricsHealthCheck : IHealthCheck
    {
        public const string HealthCheckResultDescription = "Business Metrics API Health Check";

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
        {
            return HealthCheckResult.Healthy(HealthCheckResultDescription);
        }
    }
}
