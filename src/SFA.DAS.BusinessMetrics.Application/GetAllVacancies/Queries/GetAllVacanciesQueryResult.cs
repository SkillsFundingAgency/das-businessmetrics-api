namespace SFA.DAS.BusinessMetrics.Application.GetAllVacancies.Queries
{
    public record GetAllVacanciesQueryResult
    {
        public List<string?> Vacancies { get; set; } = [];
    }
}