using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.BusinessMetrics.Api.Controllers;
using SFA.DAS.BusinessMetrics.Application.GetAllVacancies.Queries;
using SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries;
using SFA.DAS.BusinessMetrics.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.BusinessMetrics.Api.UnitTests.Controllers.Vacancy
{
    public class VacancyMetricsControllerGetAllVacanciesTests
    {
        [Test, MoqAutoData]
        public async Task GetAllVacancies_InvokesQueryHandler(
            DateTime startDate,
            DateTime endDate,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] VacancyController sut,
            GetVacancyMetricsQueryResult getMetricNamesQueryResult)
        {
            await sut.GetAll(startDate, endDate);
            mediatorMock.Verify(m => m.Send(It.IsAny<GetAllVacanciesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task GetAllVacancies_HandlerReturnsData_ReturnsOkResponse(
            DateTime startDate,
            DateTime endDate,
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] VacancyController sut,
            GetAllVacanciesQueryResult getAllVacanciesQueryResult)
        {
            var response = new ValidatedResponse<GetAllVacanciesQueryResult>(getAllVacanciesQueryResult);

            mediatorMock.Setup(m => m.Send(It.Is<GetAllVacanciesQuery>(q =>
                        q.StartDate == startDate
                        && q.EndDate == endDate),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await sut.GetAll(startDate, endDate);

            result.As<OkObjectResult>().Should().NotBeNull();
            result.As<OkObjectResult>().Value.Should().Be(getAllVacanciesQueryResult);
        }
    }
}
