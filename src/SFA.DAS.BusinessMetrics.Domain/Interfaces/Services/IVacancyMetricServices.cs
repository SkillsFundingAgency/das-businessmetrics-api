﻿using SFA.DAS.BusinessMetrics.Domain.Models;

namespace SFA.DAS.BusinessMetrics.Domain.Interfaces.Services
{
    public interface IVacancyMetricServices
    {
        Task<List<VacancyMetrics>> GetVacancyMetrics(
            DateTime startDate,
            DateTime endDate,
            CancellationToken token);
    }
}