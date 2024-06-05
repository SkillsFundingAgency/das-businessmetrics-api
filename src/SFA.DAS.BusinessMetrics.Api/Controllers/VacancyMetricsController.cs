using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.BusinessMetrics.Api.Common;
using SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries;

namespace SFA.DAS.BusinessMetrics.Api.Controllers
{
    [ApiController]
    [Route("api/vacancy")]
    public class VacancyMetricsController(ILogger<VacancyMetricsController> logger, IMediator mediator) : ActionResponseControllerBase
    {
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