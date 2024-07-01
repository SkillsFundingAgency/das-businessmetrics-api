using MediatR;
using SFA.DAS.BusinessMetrics.Application.Mediatr.Responses;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Application.GetAllVacancies.Queries
{
    public record GetAllVacanciesQueryHandler(IVacancyMetricServices MetricServices) : IRequestHandler<GetAllVacanciesQuery, ValidatedResponse<GetAllVacanciesQueryResult>>
    {
        public async Task<ValidatedResponse<GetAllVacanciesQueryResult>> Handle(GetAllVacanciesQuery request, CancellationToken cancellationToken)
        {
            var vacancies = await MetricServices.GetAllVacancies(request.StartDate, request.EndDate, cancellationToken);

            return new ValidatedResponse<GetAllVacanciesQueryResult>(new GetAllVacanciesQueryResult
            {
                Vacancies = vacancies.ToList()
            });
        }
    }
}