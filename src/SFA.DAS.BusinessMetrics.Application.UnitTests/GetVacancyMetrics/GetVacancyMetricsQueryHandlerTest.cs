using SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries;
using SFA.DAS.BusinessMetrics.Domain.Constants;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.BusinessMetrics.Domain.Models;

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

            var response = await sut.Handle(request, CancellationToken.None);

            response.Result.VacancyMetrics.Count.Should().Be(result.Count);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_WhenMetricsReturned_GroupsByVacancyReference(
            [Frozen] Mock<IVacancyMetricServices> metricServices,
            GetVacancyMetricsQueryHandler sut,
            GetVacancyMetricsQuery request)
        {
            var metrics = new List<VacancyMetrics>
            {
                new() { VacancyReference = "VAC001", Name = MetricConstants.Vacancy.Views, Count = 10 },
                new() { VacancyReference = "VAC001", Name = MetricConstants.Vacancy.Started, Count = 5 },
                new() { VacancyReference = "VAC002", Name = MetricConstants.Vacancy.Views, Count = 3 },
            };

            metricServices
                .Setup(x => x.GetVacancyMetrics(request.StartDate, request.EndDate, CancellationToken.None))
                .ReturnsAsync(metrics);

            var response = await sut.Handle(request, CancellationToken.None);

            response.Result.VacancyMetrics.Count.Should().Be(2);
            response.Result.VacancyMetrics.Should().ContainSingle(x => x.VacancyReference == "VAC001");
            response.Result.VacancyMetrics.Should().ContainSingle(x => x.VacancyReference == "VAC002");
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_WhenMetricsReturned_MapsAllMetricCountsCorrectly(
            [Frozen] Mock<IVacancyMetricServices> metricServices,
            GetVacancyMetricsQueryHandler sut,
            GetVacancyMetricsQuery request)
        {
            const string vacancyRef = "VAC001";
            var metrics = new List<VacancyMetrics>
            {
                new() { VacancyReference = vacancyRef, Name = MetricConstants.Vacancy.Views, Count = 10 },
                new() { VacancyReference = vacancyRef, Name = MetricConstants.Vacancy.Started, Count = 5 },
                new() { VacancyReference = vacancyRef, Name = MetricConstants.Vacancy.Submitted, Count = 3 },
                new() { VacancyReference = vacancyRef, Name = MetricConstants.Vacancy.SearchResults, Count = 20 },
                new() { VacancyReference = vacancyRef, Name = MetricConstants.Vacancy.Saved, Count = 7 },
            };

            metricServices
                .Setup(x => x.GetVacancyMetrics(request.StartDate, request.EndDate, CancellationToken.None))
                .ReturnsAsync(metrics);

            var response = await sut.Handle(request, CancellationToken.None);

            var vacancyMetric = response.Result.VacancyMetrics.Single();
            vacancyMetric.ViewsCount.Should().Be(10);
            vacancyMetric.ApplicationStartedCount.Should().Be(5);
            vacancyMetric.ApplicationSubmittedCount.Should().Be(3);
            vacancyMetric.SearchResultsCount.Should().Be(20);
            vacancyMetric.SavedCount.Should().Be(7);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_WhenMetricNameCasingDiffers_StillMapsCorrectly(
            [Frozen] Mock<IVacancyMetricServices> metricServices,
            GetVacancyMetricsQueryHandler sut,
            GetVacancyMetricsQuery request)
        {
            var metrics = new List<VacancyMetrics>
            {
                new() { VacancyReference = "VAC001", Name = MetricConstants.Vacancy.Views.ToUpper(), Count = 8 },
                new() { VacancyReference = "VAC001", Name = MetricConstants.Vacancy.Started.ToLower(), Count = 4 },
            };

            metricServices
                .Setup(x => x.GetVacancyMetrics(request.StartDate, request.EndDate, CancellationToken.None))
                .ReturnsAsync(metrics);

            var response = await sut.Handle(request, CancellationToken.None);

            var vacancyMetric = response.Result.VacancyMetrics.Single();
            vacancyMetric.ViewsCount.Should().Be(8);
            vacancyMetric.ApplicationStartedCount.Should().Be(4);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_WhenMetricNotPresent_DefaultsCountToZero(
            [Frozen] Mock<IVacancyMetricServices> metricServices,
            GetVacancyMetricsQueryHandler sut,
            GetVacancyMetricsQuery request)
        {
            var metrics = new List<VacancyMetrics>
            {
                new() { VacancyReference = "VAC001", Name = MetricConstants.Vacancy.Views, Count = 5 },
            };

            metricServices
                .Setup(x => x.GetVacancyMetrics(request.StartDate, request.EndDate, CancellationToken.None))
                .ReturnsAsync(metrics);

            var response = await sut.Handle(request, CancellationToken.None);

            var vacancyMetric = response.Result.VacancyMetrics.Single();
            vacancyMetric.ApplicationStartedCount.Should().Be(0);
            vacancyMetric.ApplicationSubmittedCount.Should().Be(0);
            vacancyMetric.SearchResultsCount.Should().Be(0);
            vacancyMetric.SavedCount.Should().Be(0);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_WhenNoMetricsReturned_ReturnsEmptyList(
            [Frozen] Mock<IVacancyMetricServices> metricServices,
            GetVacancyMetricsQueryHandler sut,
            GetVacancyMetricsQuery request)
        {
            metricServices
                .Setup(x => x.GetVacancyMetrics(request.StartDate, request.EndDate, CancellationToken.None))
                .ReturnsAsync([]);

            var response = await sut.Handle(request, CancellationToken.None);

            response.Result.VacancyMetrics.Should().BeEmpty();
        }

        [Test, RecursiveMoqAutoData]
        public async Task Handle_WhenMultipleMetricsExistForSameName_ReturnsFirst(
            [Frozen] Mock<IVacancyMetricServices> metricServices,
            GetVacancyMetricsQueryHandler sut,
            GetVacancyMetricsQuery request)
        {
            var metrics = new List<VacancyMetrics>
            {
                new() { VacancyReference = "VAC001", Name = MetricConstants.Vacancy.Views, Count = 10 },
                new() { VacancyReference = "VAC001", Name = MetricConstants.Vacancy.Views, Count = 99 },
            };

            metricServices
                .Setup(x => x.GetVacancyMetrics(request.StartDate, request.EndDate, CancellationToken.None))
                .ReturnsAsync(metrics);

            var response = await sut.Handle(request, CancellationToken.None);

            response.Result.VacancyMetrics.Single().ViewsCount.Should().Be(10);
        }
    }
}

