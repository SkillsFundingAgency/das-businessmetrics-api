using Microsoft.Extensions.Options;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Configuration;

namespace SFA.DAS.BusinessMetrics.Api.AppStart
{
    public static class AddConfigurationOptionsExtension
    {
        public static void AddConfigurationOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<MetricsConfiguration>(configuration.GetSection(nameof(MetricsConfiguration)));
            services.AddSingleton<IMetricsConfiguration>(serviceProvider => serviceProvider.GetRequiredService<IOptions<MetricsConfiguration>>().Value);
        }
    }
}
