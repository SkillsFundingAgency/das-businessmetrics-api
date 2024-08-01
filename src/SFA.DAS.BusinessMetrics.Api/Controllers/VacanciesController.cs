using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.BusinessMetrics.Api.Common;
using SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries;

namespace SFA.DAS.BusinessMetrics.Api.Controllers
{
    [ApiController]
    [Route("api/vacancies")]
    public class VacanciesController(
        ILogger<VacanciesController> logger,
        IMediator mediator) : ActionResponseControllerBase
    {
        [HttpGet]
        [Route("metrics")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(GetVacancyMetricsQueryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetVacancyMetrics(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            logger.LogInformation("Business Metrics API: Received query to get metrics for vacancies");

            var response = await mediator.Send(new GetVacancyMetricsQuery(startDate, endDate));

            return GetResponse(response);
        }
    }
}