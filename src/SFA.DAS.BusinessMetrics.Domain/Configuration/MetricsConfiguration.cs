using SFA.DAS.BusinessMetrics.Domain.Models;

namespace SFA.DAS.BusinessMetrics.Domain.Configuration
{
    public class MetricsConfiguration
    {
        public List<MetricsCounterConfig> CustomMetrics { get; set; } = [];
    }
}