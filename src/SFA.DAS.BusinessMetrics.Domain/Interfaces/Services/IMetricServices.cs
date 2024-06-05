namespace SFA.DAS.BusinessMetrics.Domain.Interfaces.Services
{
    public interface IMetricServices
    {
        List<string> GetMetricServiceNames();

        Task<long> GetVacancyViews(string metricName, string vacancyReference, DateTime startDate, DateTime endDate, CancellationToken token);
    }
}
