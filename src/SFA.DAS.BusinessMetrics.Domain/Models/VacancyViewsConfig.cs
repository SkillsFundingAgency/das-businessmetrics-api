namespace SFA.DAS.BusinessMetrics.Domain.Models
{
    public class VacancyViewsConfig
    {
        public required string ResourceIdentifier { get; set; }
        public required string ServiceName { get; set; }
        public required string CounterName { get; set; }
    }
}
