using SFA.DAS.BusinessMetrics.Domain.Models;

namespace SFA.DAS.BusinessMetrics.Domain.Interfaces.Configuration
{
    public interface IMetricsConfiguration
    {
        List<VacancyViewsConfig> VacancyViewsConfig { get; set; }
    }
}
