using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.BusinessMetrics.Api.Common;
using SFA.DAS.BusinessMetrics.Application.GetMetricNames.Queries;
using SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries;

namespace SFA.DAS.BusinessMetrics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController(ILogger<MetricsController> logger, IMediator mediator) : ActionResponseControllerBase
    {
        protected override string ControllerName => "metrics";

        [HttpGet]
        [Route("GetServiceNames")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetMetricNamesQueryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServiceNames()
        {
            logger.LogInformation("Business Metrics API: Received command to get Application Names");

            var response = await mediator.Send(new GetMetricNamesQuery());

            return GetResponse(response);
        }

        [HttpGet]
        [Route("{serviceName}/metrics/{vacancyReference}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetVacancyMetricsQueryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVacancyMetrics(
            [FromRoute] string serviceName,
            [FromRoute] string vacancyReference,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            logger.LogInformation("Business Metrics API: Received command to get metrics for vacancy reference: {vacancyReference}", vacancyReference);

            var response = await mediator.Send(new GetVacancyMetricsQuery(serviceName, vacancyReference, startDate, endDate));

            return GetResponse(response);
        }
    }
}