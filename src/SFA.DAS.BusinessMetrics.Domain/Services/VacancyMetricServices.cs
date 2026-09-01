using Azure.Core;
using Azure.Monitor.Query;
using Microsoft.Extensions.Options;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Constants;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.BusinessMetrics.Domain.Models;

namespace SFA.DAS.BusinessMetrics.Domain.Services
{
    public class VacancyMetricServices(
        IOptions<LogAnalyticsWorkSpace> logWorkspaceConfigurationOptions,
        ILogsQueryClient queryClient)
        : IVacancyMetricServices
    {
        private readonly LogAnalyticsWorkSpace _logAnalyticsWorkSpaceConfiguration =
            logWorkspaceConfigurationOptions.Value;

        private static readonly string VacancyMetricsQuery = string.Join(Environment.NewLine,
            $"{MetricConstants.CustomMetricsTableName}",
            $"| where Name contains '{MetricConstants.CustomDimensions.VacancyDimensionName}'",
            $"| extend CustomDimension = tostring(Properties.['{MetricConstants.CustomDimensions.VacancyReference}'])",
            $"| summarize Count = count() by CustomDimension, Name",
            $"| order by CustomDimension");

        public async Task<List<VacancyMetrics>> GetVacancyMetrics(
            DateTime startDate,
            DateTime endDate,
            CancellationToken token)
        {
            var result = await queryClient.ProcessQuery(
                new ResourceIdentifier(_logAnalyticsWorkSpaceConfiguration.Identifier),
                VacancyMetricsQuery,
                new QueryTimeRange(startDate, endDate),
                token);

            if (result is not { Rows.Count: > 0 })
                return [];

            return
            [
                .. result.Rows
                    .Select(row => new VacancyMetrics
                    {
                        VacancyReference = row[0] as string,
                        Name = row[1] as string,
                        Count = (long) row[2],
                    })
            ];
        }
    }
}