using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.BusinessMetrics.Api.Models.Requests
{
    public record GetVacancyMetricsRequest
    {
        [FromRoute]
        public required string ServiceName { get; set; }
        [FromRoute]
        public required string VacancyReference { get; set; }
        [FromQuery]
        public DateTime StartDate { get; set; }
        [FromQuery]
        public DateTime EndDate { get; set; }
    }
}
