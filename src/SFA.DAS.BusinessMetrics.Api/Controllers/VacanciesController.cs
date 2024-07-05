using MediatR;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.BusinessMetrics.Api.Common;
using SFA.DAS.BusinessMetrics.Application.GetAllVacancies.Queries;
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
            logger.LogInformation("Business Metrics API: Received query to get metrics for vacancy reference: {vacancyReference}", vacancyReference);

            var response = await mediator.Send(new GetVacancyMetricsQuery(serviceName, vacancyReference, startDate, endDate));

            return GetResponse(response);
        }

        [HttpGet]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(GetAllVacanciesQueryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            logger.LogInformation("Business Metrics API: Received query to get all vacancies");

            var response = await mediator.Send(new GetAllVacanciesQuery(startDate, endDate));

            return GetResponse(response);
        }
    }
}