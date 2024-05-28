using MediatR;
using SFA.DAS.BusinessMetrics.Application.Mediatr.Responses;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries
{
    public record GetVacancyMetricsQueryHandler(IMetricServices MetricServices) : IRequestHandler<GetVacancyMetricsQuery, ValidatedResponse<GetVacancyMetricsQueryResult>>
    {
        public async Task<ValidatedResponse<GetVacancyMetricsQueryResult>> Handle(GetVacancyMetricsQuery request, CancellationToken cancellationToken)
        {
            var metrics = await MetricServices.GetVacancyViews(request.ServiceName, request.VacancyReference, request.StartDate, request.EndDate, cancellationToken);

            return new ValidatedResponse<GetVacancyMetricsQueryResult>(new GetVacancyMetricsQueryResult
            {
                VacancyViews = metrics
            });
        }
    }
}
