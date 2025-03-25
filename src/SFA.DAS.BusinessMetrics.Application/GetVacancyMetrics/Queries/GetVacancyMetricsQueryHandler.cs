using MediatR;
using SFA.DAS.BusinessMetrics.Application.Mediatr.Responses;
using SFA.DAS.BusinessMetrics.Domain.Constants;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.BusinessMetrics.Domain.Models;

namespace SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries
{
    public record GetVacancyMetricsQueryHandler(IVacancyMetricServices MetricServices) : IRequestHandler<GetVacancyMetricsQuery, ValidatedResponse<GetVacancyMetricsQueryResult>>
    {
        public async Task<ValidatedResponse<GetVacancyMetricsQueryResult>> Handle(GetVacancyMetricsQuery request, CancellationToken cancellationToken)
        {
            var metricsResult = await MetricServices.GetVacancyMetrics(request.StartDate, request.EndDate, cancellationToken);

            var vacancyMetrics = metricsResult.GroupBy(x => x.VacancyReference)
                .Select(metrics => new GetVacancyMetricsQueryResult.VacancyMetric
                {
                    VacancyReference = metrics.Key,
                    ViewsCount = GetMetricCount(metrics, MetricConstants.Vacancy.Views),
                    ApplicationStartedCount = GetMetricCount(metrics, MetricConstants.Vacancy.Started),
                    ApplicationSubmittedCount = GetMetricCount(metrics, MetricConstants.Vacancy.Submitted),
                    SearchResultsCount = GetMetricCount(metrics, MetricConstants.Vacancy.SearchResults)
                })
                .ToList();

            return new ValidatedResponse<GetVacancyMetricsQueryResult>(new GetVacancyMetricsQueryResult
            {
                VacancyMetrics = [.. vacancyMetrics]
            });
        }

        private static long GetMetricCount(IEnumerable<VacancyMetrics> metrics, string metricName)
        {
            return metrics
                .Where(fil => fil.Name!.Contains(metricName, StringComparison.InvariantCultureIgnoreCase))
                .Select(x => x.Count)
                .FirstOrDefault();
        }
    }
}