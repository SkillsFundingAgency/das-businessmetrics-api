using MediatR;
using SFA.DAS.BusinessMetrics.Application.Mediatr.Responses;

namespace SFA.DAS.BusinessMetrics.Application.GetAllVacancies.Queries
{
    public record GetAllVacanciesQuery(DateTime StartDate, DateTime EndDate) : IRequest<ValidatedResponse<GetAllVacanciesQueryResult>>;
}