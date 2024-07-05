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
            var viewMetricsTask = MetricServices.GetVacancyMetrics(request.ServiceName, MetricConstants.Vacancy.VacancyViews, request.VacancyReference, request.StartDate, request.EndDate, cancellationToken);
            var startedMetricsTask = MetricServices.GetVacancyMetrics(request.ServiceName, MetricConstants.Vacancy.VacancyStarted, request.VacancyReference, request.StartDate, request.EndDate, cancellationToken);
            var submittedMetricsTask = MetricServices.GetVacancyMetrics(request.ServiceName, MetricConstants.Vacancy.VacancySubmitted, request.VacancyReference, request.StartDate, request.EndDate, cancellationToken);
            var searchResultsMetricsTask = MetricServices.GetVacancyMetrics(request.ServiceName, MetricConstants.Vacancy.VacancyInSearchResults, request.VacancyReference, request.StartDate, request.EndDate, cancellationToken);

            await Task.WhenAll(viewMetricsTask, startedMetricsTask, submittedMetricsTask, searchResultsMetricsTask);

            return new ValidatedResponse<GetVacancyMetricsQueryResult>(new GetVacancyMetricsQueryResult
            {
                ViewsCount = viewMetricsTask.Result,
                ApplicationStartedCount = startedMetricsTask.Result,
                ApplicationSubmittedCount = submittedMetricsTask.Result,
                SearchResultsCount = searchResultsMetricsTask.Result
            });
        }
    }
}