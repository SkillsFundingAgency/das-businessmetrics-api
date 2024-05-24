using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.BusinessMetrics.Application.GetMetricNames.Queries;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.BusinessMetrics.Application.UnitTests.GetMetricNames.Queries
{
    public class GetMetricNamesQueryHandlerTest
    {
        [Test, RecursiveMoqAutoData]
        public async Task Handle_Returns_ServiceNames(
            [Frozen] Mock<IMetricServices> metricServices,
            GetMetricNamesQueryHandler sut,
            List<string> serviceNames)
        {
            metricServices.Setup(a => a.GetMetricServiceNames()).Returns(serviceNames);

            var response = await sut.Handle(new GetMetricNamesQuery(), new CancellationToken());
            
            response.Result.ServiceNames.Should().BeEquivalentTo(serviceNames);
            response.Result.ServiceNames.Count.Should().Be(serviceNames.Count);
        }
    }
}
