using MediatR;
using SFA.DAS.BusinessMetrics.Application.Mediatr.Responses;

namespace SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries
{
    public record GetVacancyMetricsQuery(
        string ServiceName,
        string VacancyReference,
        DateTime StartDate,
        DateTime EndDate) : IRequest<ValidatedResponse<GetVacancyMetricsQueryResult>>;
}
