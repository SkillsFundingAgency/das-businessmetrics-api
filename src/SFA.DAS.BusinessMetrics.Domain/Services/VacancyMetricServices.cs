using Azure.Core;
using Azure.Monitor.Query;
using Microsoft.Extensions.Options;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Domain.Services
{
    public class VacancyMetricServices(
        IOptions<MetricsConfiguration> metricsConfigurationOptions,
        IOptions<LogAnalyticsWorkSpace> logWorkspaceConfigurationOptions,
        ILogsQueryClient queryClient)
        : IVacancyMetricServices
    {
        private readonly MetricsConfiguration _metricConfiguration = metricsConfigurationOptions.Value;
        private readonly LogAnalyticsWorkSpace _logAnalyticsWorkSpaceConfiguration = logWorkspaceConfigurationOptions.Value;

        public async Task<long> GetVacancyMetrics(
            string serviceName,
            string action,
            string vacancyReference,
            DateTime startDate,
            DateTime endDate,
            CancellationToken token)
        {
            var counterName = GetCounterName(serviceName, action);

            var result = await queryClient.ProcessQuery(
                new ResourceIdentifier(_logAnalyticsWorkSpaceConfiguration.Identifier),
                $"{Constants.MetricConstants.CustomMetricsTableName} " +
                $"| where Name == '{counterName}'" +
                $"| where Properties.['{Constants.MetricConstants.CustomDimensions.VacancyReference}'] == '{vacancyReference}'" +
                $"| summarize sum(ItemCount)",
                new QueryTimeRange(startDate, endDate),
                token);

            if (result is { Rows.Count: > 0 })
            {
                return result.Rows[0].GetInt64("sum_ItemCount") ?? 0;
            }

            return 0;
        }

        public async Task<List<string?>> GetAllVacancies(DateTime startDate, DateTime endDate,
            CancellationToken token)
        {
            var filter = BuildQueryFilter();

            var result = await queryClient.ProcessQuery(
                new ResourceIdentifier(_logAnalyticsWorkSpaceConfiguration.Identifier),
                $"{Constants.MetricConstants.CustomMetricsTableName} " +
                $"| {filter} " +
                $"| project Properties.['{Constants.MetricConstants.CustomDimensions.VacancyReference}']",
                new QueryTimeRange(startDate, endDate),
                token);

            return result is not {Rows.Count: > 0} 
                ? [] 
                : result.Rows.Select(resultRow => Convert.ToString(resultRow[0]))
                    .Where(vacancyReference => !string.IsNullOrEmpty(vacancyReference))
                    .Distinct()
                    .ToList();
        }

        private string BuildQueryFilter()
        {
            var counterNames = _metricConfiguration.CustomMetrics.Select(fil => fil.CounterName).ToList();

            return counterNames.Aggregate("where ", (current, counterName) => current + (counterNames.IndexOf(counterName) == counterNames.Count - 1
                ? $"Name == '{counterName}' "
                : $"Name == '{counterName}' or "));
        }

        private string GetCounterName(string serviceName, string action)
        {
            var config = _metricConfiguration.CustomMetrics.Find(fil =>
                fil.ServiceName.Equals(serviceName, StringComparison.InvariantCultureIgnoreCase)
                && fil.Action.Equals(action, StringComparison.InvariantCultureIgnoreCase));

            return config is not null ? config.CounterName : string.Empty;
        }
    }
}