using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SFA.DAS.BusinessMetrics.Api.Controllers;
using SFA.DAS.BusinessMetrics.Application.GetMetricNames.Queries;
using SFA.DAS.BusinessMetrics.Application.Mediatr.Responses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.BusinessMetrics.Api.UnitTests.Controllers.Metrics
{
    public class MetricsControllerGetServiceNamesTests
    {
        [Test, MoqAutoData]
        public async Task GetServiceNames_InvokesQueryHandler(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] MetricsController sut)
        {
            await sut.GetServiceNames();
            mediatorMock.Verify(m => m.Send(It.IsAny<GetMetricNamesQuery>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        [MoqAutoData]
        public async Task GetServiceNames_HandlerReturnsData_ReturnsOkResponse(
            [Frozen] Mock<IMediator> mediatorMock,
            [Greedy] MetricsController sut,
            GetMetricNamesQueryResult getMetricNamesQueryResult)
        {
            var response =
                new ValidatedResponse<GetMetricNamesQueryResult>(getMetricNamesQueryResult);

            mediatorMock.Setup(m => m.Send(It.IsAny<GetMetricNamesQuery>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await sut.GetServiceNames();

            result.As<OkObjectResult>().Should().NotBeNull();
            result.As<OkObjectResult>().Value.Should().Be(getMetricNamesQueryResult);
        }
    }
}
