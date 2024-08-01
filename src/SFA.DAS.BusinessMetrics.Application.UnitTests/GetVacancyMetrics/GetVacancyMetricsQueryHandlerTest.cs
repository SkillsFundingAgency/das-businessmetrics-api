using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.BusinessMetrics.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.BusinessMetrics.Application.UnitTests.GetVacancyMetrics
{
    public class GetVacancyMetricsQueryHandlerTest
    {
        [Test, RecursiveMoqAutoData]
        public async Task Handle_Returns_Metrics(
            [Frozen] Mock<IVacancyMetricServices> metricServices,
            GetVacancyMetricsQueryHandler sut,
            GetVacancyMetricsQuery request,
            List<VacancyMetrics> result)
        {
            metricServices.Setup(a => a.GetVacancyMetrics(request.StartDate, request.EndDate, CancellationToken.None)).ReturnsAsync(result);

            var response = await sut.Handle(request, new CancellationToken());

            response.Result.VacancyMetrics.Count.Should().Be(result.Count);
        }
    }
}
