using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace SFA.DAS.BusinessMetrics.Domain.Interfaces.Services
{
    public interface IHealthCheckServices
    {
        Task<HealthCheckResult> GetHealthCheck(CancellationToken token);
    }
}