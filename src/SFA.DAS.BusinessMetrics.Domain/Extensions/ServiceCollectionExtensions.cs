using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.BusinessMetrics.Domain.Services;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.BusinessMetrics.Domain.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddTransient<ILogsQueryClient, AzureMonitorLogsQueryClient>();
            services.AddTransient<IMetricServices, MetricServices>();
            services.AddTransient<IVacancyMetricServices, VacancyMetricServices>();
            services.AddTransient<IHealthCheckServices, HealthCheckServices>();

            return services;
        }
    }
}
