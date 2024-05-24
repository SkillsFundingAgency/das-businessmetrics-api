using SFA.DAS.BusinessMetrics.Domain.Interfaces.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Models;

namespace SFA.DAS.BusinessMetrics.Domain.Configuration
{
    public class MetricsConfiguration : IMetricsConfiguration
    {
        public List<VacancyViewsConfig> VacancyViewsConfig { get; set; } = [];
    }
}
