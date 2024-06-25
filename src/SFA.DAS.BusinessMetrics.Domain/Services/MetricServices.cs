using Microsoft.Extensions.Options;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Domain.Services
{
    public class MetricServices(IOptions<ServicesConfiguration> servicesConfigurationOptions) : IMetricServices
    {
        private readonly ServicesConfiguration _servicesConfiguration = servicesConfigurationOptions.Value;

        public List<string> GetMetricServiceNames()
        {
            return _servicesConfiguration.Resources.Select(fil => fil.ServiceName).ToList();
        }
    }
}