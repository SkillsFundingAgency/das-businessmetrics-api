namespace SFA.DAS.BusinessMetrics.Domain.Models
{
    public record VacancyMetrics
    {
        public string? VacancyReference { get; init; }
        public string? Name { get; init; }
        public long Count { get; init; } = 0;
    }
}