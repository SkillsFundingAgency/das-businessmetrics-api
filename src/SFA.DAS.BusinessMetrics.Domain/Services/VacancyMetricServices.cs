using Azure.Core;
using Azure.Monitor.Query;
using Microsoft.Extensions.Options;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Domain.Services
{
    public class VacancyMetricServices(
        IOptions<MetricsConfiguration> metricsConfigurationOptions,
        IOptions<ServicesConfiguration> servicesConfigurationOptions,
        ILogsQueryClient queryClient)
        : IVacancyMetricServices
    {
        private readonly MetricsConfiguration _metricConfiguration = metricsConfigurationOptions.Value;
        private readonly ServicesConfiguration _servicesConfiguration = servicesConfigurationOptions.Value;

        public async Task<long> GetVacancyMetrics(
            string serviceName,
            string action,
            string vacancyReference,
            DateTime startDate,
            DateTime endDate,
            CancellationToken token)
        {
            var resourceIdentifier = GetResourceIdentifier(serviceName);
            var counterName = GetCounterName(serviceName, action);

            var result = await queryClient.ProcessQuery(
                new ResourceIdentifier(resourceIdentifier),
                $"{Constants.MetricConstants.CustomMetricsTableName} " +
                $"| where name == '{counterName}'" +
                $"| where customDimensions.['{Constants.MetricConstants.CustomDimensions.VacancyReference}'] == '{vacancyReference}'" +
                $"| summarize sum(value)",
                new QueryTimeRange(startDate, endDate),
                token);

            if (result is { Rows.Count: > 0 })
            {
                return result.Rows.FirstOrDefault()!.GetInt64("sum_value") ?? 0;
            }

            return 0;
        }

        private string GetResourceIdentifier(string serviceName)
        {
            var config = _servicesConfiguration.Resources
                .FirstOrDefault(fil => fil.ServiceName.Equals(serviceName, StringComparison.CurrentCultureIgnoreCase));

            return config is not null ? config.ResourceIdentifier : string.Empty;
        }

        private string GetCounterName(string serviceName, string action)
        {
            var config = _metricConfiguration.CustomMetrics.FirstOrDefault(fil =>
                fil.ServiceName.Equals(serviceName, StringComparison.InvariantCultureIgnoreCase)
                && fil.Action.Equals(action, StringComparison.InvariantCultureIgnoreCase));

            return config is not null ? config.CounterName : string.Empty;
        }
    }
}