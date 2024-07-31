using Azure.Core;
using Azure.Monitor.Query;
using Microsoft.Extensions.Options;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.BusinessMetrics.Domain.Models;

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

        public async Task<List<VacancyMetrics>> GetVacancyMetrics(
            string serviceName,
            DateTime startDate,
            DateTime endDate,
            CancellationToken token)
        {
            var counterName = GetCounterName(serviceName);

            var result = await queryClient.ProcessQuery(
                new ResourceIdentifier(_logAnalyticsWorkSpaceConfiguration.Identifier),
                $"{Constants.MetricConstants.CustomMetricsTableName} " +
                $"| where Name contains '{counterName}'" +
                $"| extend CustomDimension = tostring(Properties.['{Constants.MetricConstants.CustomDimensions.VacancyReference}'])" +
                $"| summarize Count = count() by CustomDimension, Name" +
                $"| order by CustomDimension",
                new QueryTimeRange(startDate, endDate),
                token);

            return result is not { Rows.Count: > 0 }
                ? []
                : result.Rows.Select(row => new VacancyMetrics
                {
                    VacancyReference = Convert.ToString(row[0]),
                    Name = Convert.ToString(row[1]),
                    Count = Convert.ToInt64(row[2]),
                }).ToList();
        }
        
        private string GetCounterName(string serviceName)
        {
            var config = _metricConfiguration.CustomMetrics.Find(fil =>
                fil.ServiceName.Equals(serviceName, StringComparison.InvariantCultureIgnoreCase));

            return config is not null ? config.CounterName : string.Empty;
        }
    }
}