using MediatR;
using SFA.DAS.BusinessMetrics.Application.Mediatr.Responses;
using SFA.DAS.BusinessMetrics.Domain.Constants;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries
{
    public record GetVacancyMetricsQueryHandler(IVacancyMetricServices MetricServices) : IRequestHandler<GetVacancyMetricsQuery, ValidatedResponse<GetVacancyMetricsQueryResult>>
    {
        public async Task<ValidatedResponse<GetVacancyMetricsQueryResult>> Handle(GetVacancyMetricsQuery request, CancellationToken cancellationToken)
        {
            var metricsResult = await MetricServices.GetVacancyMetrics(request.ServiceName, request.StartDate, request.EndDate, cancellationToken);
            
            var vacancyMetrics = metricsResult.GroupBy(x => x.VacancyReference)
                .Select(metrics => new GetVacancyMetricsQueryResult.VacancyMetric
                {
                    VacancyReference = metrics.Key,
                    ViewsCount =
                        metrics.Any(fil => fil.Name!.Contains($"{MetricConstants.Vacancy.Views}",
                            StringComparison.InvariantCultureIgnoreCase))
                            ? metrics.Where(fil => fil.Name!.Contains($"{MetricConstants.Vacancy.Views}",
                                StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Count).FirstOrDefault()
                            : 0,
                    ApplicationStartedCount =
                        metrics.Any(fil => fil.Name!.Contains($"{MetricConstants.Vacancy.Started}",
                            StringComparison.InvariantCultureIgnoreCase))
                            ? metrics.Where(fil => fil.Name!.Contains($"{MetricConstants.Vacancy.Started}",
                                StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Count).FirstOrDefault()
                            : 0,
                    ApplicationSubmittedCount =
                        metrics.Any(fil => fil.Name!.Contains($"{MetricConstants.Vacancy.Submitted}",
                            StringComparison.InvariantCultureIgnoreCase))
                            ? metrics.Where(fil => fil.Name!.Contains($"{MetricConstants.Vacancy.Submitted}",
                                StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Count).FirstOrDefault()
                            : 0,
                    SearchResultsCount =
                        metrics.Any(fil => fil.Name!.Contains($"{MetricConstants.Vacancy.SearchResults}",
                            StringComparison.InvariantCultureIgnoreCase))
                            ? metrics.Where(fil => fil.Name!.Contains($"{MetricConstants.Vacancy.SearchResults}",
                                StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Count).FirstOrDefault()
                            : 0,
                })
                .ToList();

            return new ValidatedResponse<GetVacancyMetricsQueryResult>(new GetVacancyMetricsQueryResult
            {
               VacancyMetrics = vacancyMetrics.ToList()
            });
        }
    }
}