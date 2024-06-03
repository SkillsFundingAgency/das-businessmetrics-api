using MediatR;
using SFA.DAS.BusinessMetrics.Application.Mediatr.Responses;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Application.GetMetricNames.Queries
{
    public record GetMetricNamesQueryHandler(IMetricServices MetricServices) : IRequestHandler<GetMetricNamesQuery, ValidatedResponse<GetMetricNamesQueryResult>>
    {
        public async Task<ValidatedResponse<GetMetricNamesQueryResult>> Handle(GetMetricNamesQuery request, CancellationToken cancellationToken)
        {
            var names = MetricServices.GetMetricServiceNames();

            return await Task.FromResult(new ValidatedResponse<GetMetricNamesQueryResult>(new GetMetricNamesQueryResult
            {
                ServiceNames = names
            }));
        }
    }
}