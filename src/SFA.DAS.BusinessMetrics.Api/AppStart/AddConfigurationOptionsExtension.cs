using SFA.DAS.BusinessMetrics.Domain.Configuration;

namespace SFA.DAS.BusinessMetrics.Api.AppStart
{
    public static class AddConfigurationOptionsExtension
    {
        public static void AddConfigurationOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<MetricsConfiguration>(configuration.GetSection(nameof(MetricsConfiguration)));
            services.Configure<ServicesConfiguration>(configuration.GetSection(nameof(ServicesConfiguration)));
        }
    }
}
