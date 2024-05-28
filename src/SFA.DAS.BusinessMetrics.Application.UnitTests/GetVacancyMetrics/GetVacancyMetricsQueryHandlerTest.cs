using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.BusinessMetrics.Application.UnitTests.GetVacancyMetrics
{
    public class GetVacancyMetricsQueryHandlerTest
    {
        [Test, RecursiveMoqAutoData]
        public async Task Handle_Returns_Metrics(
            [Frozen] Mock<IMetricServices> metricServices,
            GetVacancyMetricsQueryHandler sut,
            GetVacancyMetricsQuery request,
            int metricsCount)
        {
            metricServices.Setup(a => a.GetVacancyViews(request.ServiceName, request.VacancyReference, request.StartDate, request.EndDate, CancellationToken.None)).ReturnsAsync(metricsCount);

            var response = await sut.Handle(request, new CancellationToken());

            response.Result.VacancyViews.Should().Be(metricsCount);
        }
    }
}
