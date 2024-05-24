using Azure.Core;
using Azure.Monitor.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using ConfigurationErrorsException = System.Configuration.ConfigurationErrorsException;

namespace SFA.DAS.BusinessMetrics.Domain.Services
{
    public class VacancyMetricServices(
        ILogger<VacancyMetricServices> logger,
        IOptions<MetricsConfiguration> metricConfigurationOptions, ILogsQueryClient queryClient)
        : IMetricServices
    {
        private readonly MetricsConfiguration _metricConfiguration = metricConfigurationOptions.Value;

        public List<string> GetMetricServiceNames()
        {
            return _metricConfiguration.VacancyViewsConfig.Select(fil => fil.ServiceName).ToList();
        }

        public async Task<long> GetVacancyMetrics(string serviceName, string vacancyReference, DateTime startDate, DateTime endDate, CancellationToken token)
        {
            var config = _metricConfiguration.VacancyViewsConfig
                .SingleOrDefault(fil => fil.ServiceName.Equals(serviceName, StringComparison.CurrentCultureIgnoreCase));

            if (config is null)
            {
                logger.LogError("Configuration could not be found for service name: {serviceName}", serviceName);
                throw new ConfigurationErrorsException($"Configuration could not be found for service name: {serviceName}");
            }

            var result = await queryClient.ProcessQuery(
                new ResourceIdentifier(config.ResourceIdentifier),
                $"{Constants.MetricConstants.CustomMetricsTableName} " +
                $"| where name == '{config.CounterName}'" +
                $"| where customDimensions.['{Constants.MetricConstants.CustomDimensions.VacancyReference}'] == '{vacancyReference}'" +
                $"| summarize sum(value)",
                new QueryTimeRange(startDate, endDate),
                token);

            if (result.Rows is { Count: > 0 })
            {
                return result.Rows.FirstOrDefault()!.GetInt64("sum_value") ?? 0;
            }

            return 0;
        }
    }
}
