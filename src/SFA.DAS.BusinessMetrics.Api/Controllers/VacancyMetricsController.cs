using Microsoft.AspNetCore.Mvc;
using SFA.DAS.BusinessMetrics.Api.Common;

namespace SFA.DAS.BusinessMetrics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VacancyMetricsController(ILogger<VacancyMetricsController> logger) : ActionResponseControllerBase
    {
        protected override string ControllerName => "VacancyMetrics";

        private readonly ILogger<VacancyMetricsController> _logger = logger;
    }
}
