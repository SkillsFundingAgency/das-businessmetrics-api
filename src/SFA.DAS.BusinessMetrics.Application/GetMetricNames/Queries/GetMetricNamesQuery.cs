using MediatR;
using SFA.DAS.BusinessMetrics.Application.Mediatr.Responses;

namespace SFA.DAS.BusinessMetrics.Application.GetMetricNames.Queries
{
    public record GetMetricNamesQuery : IRequest<ValidatedResponse<GetMetricNamesQueryResult>>;
}
