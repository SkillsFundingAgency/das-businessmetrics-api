namespace SFA.DAS.BusinessMetrics.Domain.Models
{
    public class MetricsCounterConfig
    {
        public required string Action { get; set; }
        public required string ServiceName { get; set; }
        public required string CounterName { get; set; }
    }
}