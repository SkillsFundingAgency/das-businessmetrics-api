using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.BusinessMetrics.Api.AppStart;
using SFA.DAS.BusinessMetrics.Application.Extensions;
using SFA.DAS.BusinessMetrics.Application.GetMetricNames.Queries;
using SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries;
using SFA.DAS.BusinessMetrics.Application.Mediatr.Responses;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;

namespace SFA.DAS.BusinessMetrics.Api.UnitTests.AppStart
{
    public class WhenAddingServicesToTheContainer
    {
        [TestCase(typeof(ILogsQueryClient))]
        [TestCase(typeof(IMetricServices))]
        [TestCase(typeof(IRequestHandler<GetVacancyMetricsQuery, ValidatedResponse<GetVacancyMetricsQueryResult>>))]
        [TestCase(typeof(IRequestHandler<GetMetricNamesQuery, ValidatedResponse<GetMetricNamesQueryResult>>))]
        public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
        {
            var serviceCollection = new ServiceCollection();
            SetupServiceCollection(serviceCollection);
            var provider = serviceCollection.BuildServiceProvider();

            var type = provider.GetService(toResolve);
            Assert.That(type, Is.Not.Null);
        }

        private static void SetupServiceCollection(ServiceCollection serviceCollection)
        {
            var configuration = GenerateConfiguration();
            serviceCollection.AddSingleton(Mock.Of<IWebHostEnvironment>());
            serviceCollection.AddSingleton(Mock.Of<IConfiguration>());
            serviceCollection.AddConfigurationOptions(configuration);
            serviceCollection.AddDistributedMemoryCache();
            serviceCollection.AddApplicationRegistrations();
            serviceCollection.AddLogging();

        }

        private static IConfigurationRoot GenerateConfiguration()
        {
            var configSource = new MemoryConfigurationSource
            {
                InitialData = new List<KeyValuePair<string, string>>
            {
                new("EnvironmentName", "test"),
                new("ConnectionString", "test"),
            }!
            };

            var provider = new MemoryConfigurationProvider(configSource);

            return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
        }
    }
}
