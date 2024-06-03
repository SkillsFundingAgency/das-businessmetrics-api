namespace SFA.DAS.BusinessMetrics.Application.GetMetricNames.Queries
{
    public record GetMetricNamesQueryResult
    {
        public List<string> ServiceNames { get; init; } = [];
    }
}