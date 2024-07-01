using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries;
using SFA.DAS.BusinessMetrics.Domain.Constants;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
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
            int metricsViewsCount,
            int metricsStartedCount,
            int metricsSubmittedCount,
            int metricsSearchResultsCount)
        {
            metricServices.Setup(a => a.GetVacancyMetrics(request.ServiceName, MetricConstants.Vacancy.VacancyViews, request.VacancyReference, request.StartDate, request.EndDate, CancellationToken.None)).ReturnsAsync(metricsViewsCount);
            metricServices.Setup(a => a.GetVacancyMetrics(request.ServiceName, MetricConstants.Vacancy.VacancyStarted, request.VacancyReference, request.StartDate, request.EndDate, CancellationToken.None)).ReturnsAsync(metricsStartedCount);
            metricServices.Setup(a => a.GetVacancyMetrics(request.ServiceName, MetricConstants.Vacancy.VacancySubmitted, request.VacancyReference, request.StartDate, request.EndDate, CancellationToken.None)).ReturnsAsync(metricsSubmittedCount);
            metricServices.Setup(a => a.GetVacancyMetrics(request.ServiceName, MetricConstants.Vacancy.VacancyInSearchResults, request.VacancyReference, request.StartDate, request.EndDate, CancellationToken.None)).ReturnsAsync(metricsSearchResultsCount);

            var response = await sut.Handle(request, new CancellationToken());

            response.Result.ViewsCount.Should().Be(metricsViewsCount);
            response.Result.ApplicationStartedCount.Should().Be(metricsStartedCount);
            response.Result.ApplicationSubmittedCount.Should().Be(metricsSubmittedCount);
            response.Result.SearchResultsCount.Should().Be(metricsSearchResultsCount);
        }
    }
}
