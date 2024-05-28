using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.BusinessMetrics.Api.Controllers;
using SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries;
using SFA.DAS.BusinessMetrics.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.BusinessMetrics.Api.UnitTests.Controllers.Vacancy
{
    public class VacancyMetricsControllerGetVacancyMetricsTests
    {
        [Test]
        [MoqAutoData]
        public async Task GetVacancyMetrics_HandlerReturnsData_ReturnsOkResponse(
            string serviceName,
            string vacancyReference,
            DateTime startDate,
            DateTime endDate,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] VacancyMetricsController sut,
            GetVacancyMetricsQueryResult getMetricNamesQueryResult)
        {
            var response = new ValidatedResponse<GetVacancyMetricsQueryResult>(getMetricNamesQueryResult);

            mediatorMock.Setup(m => m.Send(It.Is<GetVacancyMetricsQuery>(q =>
                        q.ServiceName == serviceName
                        && q.VacancyReference == vacancyReference
                        && q.StartDate == startDate
                        && q.EndDate == endDate),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await sut.GetVacancyMetrics(serviceName, vacancyReference, startDate, endDate);

            result.As<OkObjectResult>().Should().NotBeNull();
            result.As<OkObjectResult>().Value.Should().Be(getMetricNamesQueryResult);
        }
    }
}
