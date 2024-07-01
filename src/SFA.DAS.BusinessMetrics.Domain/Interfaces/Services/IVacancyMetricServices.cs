namespace SFA.DAS.BusinessMetrics.Domain.Interfaces.Services
{
    public interface IVacancyMetricServices
    {
        Task<long> GetVacancyMetrics(
            string serviceName,
            string action,
            string vacancyReference,
            DateTime startDate,
            DateTime endDate,
            CancellationToken token);

        Task<List<string?>> GetAllVacancies(DateTime startDate,
            DateTime endDate,
            CancellationToken token);
    }
}