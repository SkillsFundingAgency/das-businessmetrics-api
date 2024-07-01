using Microsoft.Extensions.Options;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Domain.Services
{
    public class MetricServices(IOptions<MetricsConfiguration> metricsConfigurationOptions) : IMetricServices
    {
        private readonly MetricsConfiguration _metricsConfiguration = metricsConfigurationOptions.Value;

        public List<string> GetMetricServiceNames()
        {
            return _metricsConfiguration.CustomMetrics.Select(fil => fil.ServiceName).Distinct().ToList();
        }
    }
}