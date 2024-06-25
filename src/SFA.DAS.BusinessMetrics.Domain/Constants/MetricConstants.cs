namespace SFA.DAS.BusinessMetrics.Domain.Constants
{
    public static class MetricConstants
    {
        public const string CustomMetricsTableName = "customMetrics";

        public class CustomDimensions
        {
            public const string VacancyReference = "vacancy.reference";
        }

        public class Vacancy
        {
            public const string VacancyViews = "VacancyViews";
            public const string VacancyStarted = "VacancyStarted";
            public const string VacancySubmitted = "VacancySubmitted";
            public const string VacancyInSearchResults = "VacancyInSearchResults";
        }
    }
}