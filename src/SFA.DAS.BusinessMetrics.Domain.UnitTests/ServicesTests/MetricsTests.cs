using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.BusinessMetrics.Domain.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.BusinessMetrics.Domain.UnitTests.ServicesTests
{
    public class MetricsTests
    {
        [Test, MoqAutoData]
        public void GetMetricServiceNames_Returns_ServiceNames(
            ServicesConfiguration servicesConfiguration,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<ILogger<VacancyMetricServices>> mockLogger,
            [Frozen] Mock<IOptions<ServicesConfiguration>> mockOptions)
        {
            mockOptions.Setup(ap => ap.Value).Returns(servicesConfiguration);
            var sut = new MetricServices(mockOptions.Object);

            var actual = sut.GetMetricServiceNames();

            actual.Should().NotBeNull();
            actual.Count.Should().Be(servicesConfiguration.Resources.Count);
        }
    }
}
