using Azure.Core;
using Azure.Identity;
using Azure.Monitor.Query;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.BusinessMetrics.Domain.Services;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.BusinessMetrics.Domain.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        private const int MaxRetries = 2;
        private static readonly TimeSpan NetworkTimeout = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan RetryDelay = TimeSpan.FromMilliseconds(100);

        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton(_ => new LogsQueryClient(BuildCredential()));
            services.AddTransient<ILogsQueryClient, AzureMonitorLogsQueryClient>();
            services.AddTransient<IMetricServices, MetricServices>();
            services.AddTransient<IVacancyMetricServices, VacancyMetricServices>();
            services.AddTransient<IHealthCheckServices, HealthCheckServices>();
        }

        private static ChainedTokenCredential BuildCredential()
        {
            return new ChainedTokenCredential(
                new ManagedIdentityCredential(options: WithRetry(new ManagedIdentityCredentialOptions())),
                new AzureCliCredential(options: WithRetry(new AzureCliCredentialOptions())),
                new VisualStudioCredential(options: WithRetry(new VisualStudioCredentialOptions())),
                new VisualStudioCodeCredential(options: WithRetry(new VisualStudioCodeCredentialOptions())));

            static T WithRetry<T>(T options) where T : ClientOptions
            {
                options.Retry.NetworkTimeout = NetworkTimeout;
                options.Retry.MaxRetries = MaxRetries;
                options.Retry.Delay = RetryDelay;
                return options;
            }
        }
    }
}