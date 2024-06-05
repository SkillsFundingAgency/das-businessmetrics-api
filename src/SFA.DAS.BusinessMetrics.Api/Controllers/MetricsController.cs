using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.BusinessMetrics.Api.Common;
using SFA.DAS.BusinessMetrics.Application.GetMetricNames.Queries;

namespace SFA.DAS.BusinessMetrics.Api.Controllers
{
    [ApiController]
    [Route("api/metrics")]
    public class MetricsController(ILogger<MetricsController> logger, IMediator mediator) : ActionResponseControllerBase
    {
        [HttpGet]
        [Route("serviceNames")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetMetricNamesQueryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServiceNames()
        {
            logger.LogInformation("Business Metrics API: Received command to get Application Names");

            var response = await mediator.Send(new GetMetricNamesQuery());

            return GetResponse(response);
        }
    }
}