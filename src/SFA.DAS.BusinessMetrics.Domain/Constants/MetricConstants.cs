namespace SFA.DAS.BusinessMetrics.Domain.Constants
{
    public static class MetricConstants
    {
        public const string CustomMetricsTableName = "AppMetrics";

        public static class CustomDimensions
        {
            public const string VacancyReference = "vacancy.reference";
        }

        public static class Vacancy
        {
            public const string Views = "vacancyReference.views";
            public const string Started = "vacancyReference.started";
            public const string Submitted = "vacancyReference.submitted";
            public const string SearchResults = "vacancyReference.search";
        }
    }
}