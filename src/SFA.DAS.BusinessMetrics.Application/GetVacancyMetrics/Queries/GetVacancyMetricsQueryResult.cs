namespace SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries
{
    public record GetVacancyMetricsQueryResult
    {
        public long ViewsCount { get; init; }
        public long SearchResultsCount { get; init; }
        public long ApplicationStartedCount { get; init; }
        public long ApplicationSubmittedCount { get; init; }
    }
}
