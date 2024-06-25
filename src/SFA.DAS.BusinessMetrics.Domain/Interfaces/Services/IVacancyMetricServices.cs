namespace SFA.DAS.BusinessMetrics.Domain.Interfaces.Services
{
    public interface IVacancyMetricServices
    {
        Task<long> GetVacancyMetrics(
            string resourceIdentifier,
            string counterName,
            string vacancyReference,
            DateTime startDate,
            DateTime endDate,
            CancellationToken token);
    }
}