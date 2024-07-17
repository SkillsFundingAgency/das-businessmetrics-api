using Microsoft.Extensions.Diagnostics.HealthChecks;
using SFA.DAS.BusinessMetrics.Api.HealthCheck;

namespace SFA.DAS.BusinessMetrics.Api.AppStart
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceHealthChecks(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddCheck<BusinessMetricsHealthCheck>(BusinessMetricsHealthCheck.HealthCheckResultDescription,
                    failureStatus: HealthStatus.Unhealthy,
                    tags: new[] { "Ready" })
                .AddCheck<LogAnalyticsWorkspaceHealthCheck>(LogAnalyticsWorkspaceHealthCheck.HealthCheckResultDescription,
                    failureStatus: HealthStatus.Degraded,
                    tags: new[] { "Ready" });

            return services;
        }
    }
}
