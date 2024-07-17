using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Api.HealthCheck
{
    public class LogAnalyticsWorkspaceHealthCheck(IHealthCheckServices services) : IHealthCheck
    {
        public const string HealthCheckResultDescription = "Log analytics workspace connection";

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
            CancellationToken cancellationToken = new CancellationToken())
        {

            var result = await services.GetHealthCheck(cancellationToken);

            return result.Equals(HealthCheckResult.Healthy()) ? HealthCheckResult.Healthy(HealthCheckResultDescription) : HealthCheckResult.Degraded(HealthCheckResultDescription);
        } 
    }
}