using FluentValidation.TestHelper;
using SFA.DAS.BusinessMetrics.Application.GetVacancyMetrics.Queries;

namespace SFA.DAS.BusinessMetrics.Application.UnitTests.GetVacancyMetrics
{
    public class GetVacancyMetricsQueryValidatorTest
    {
        [TestCase("1000013242", true)]
        [TestCase("12345678", false)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        public async Task Validates_VacancyReference_NotNull_Not_Empty(string vacancyReference, bool isValid)
        {
            var query = new GetVacancyMetricsQuery("", vacancyReference, DateTime.Now, DateTime.Now);
            var sut = new GetVacancyMetricsQueryValidator();
            var result = await sut.TestValidateAsync(query);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.VacancyReference);
            else
            {
                result.ShouldHaveValidationErrorFor(c => c.VacancyReference)
                    .WithErrorMessage(GetVacancyMetricsQueryValidator.VacancyReferenceTooShort);
            }
        }

        [TestCase("some name", true)]
        [TestCase("", false)]
        [TestCase(" ", false)]
        public async Task Validates_ServiceName_NotNull_Not_Empty(string serviceName, bool isValid)
        {
            var query = new GetVacancyMetricsQuery(serviceName, "1000013242", DateTime.Now, DateTime.Now);
            var sut = new GetVacancyMetricsQueryValidator();
            var result = await sut.TestValidateAsync(query);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.ServiceName);
            else
            {
                result.ShouldHaveValidationErrorFor(c => c.ServiceName);
            }
        }

        [TestCase("1 jan 2024", "10 jan 2024",  true)]
        [TestCase("1 jan 2024", "31 dec 2023", false)]
        public async Task Validates_ServiceName_NotNull_Not_Empty(string startDate, string endDate, bool isValid)
        {
            var query = new GetVacancyMetricsQuery("some name", "1000013242", Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            var sut = new GetVacancyMetricsQueryValidator();
            var result = await sut.TestValidateAsync(query);

            if (isValid)
                result.ShouldNotHaveValidationErrorFor(c => c.EndDate);
            else
            {
                result.ShouldHaveValidationErrorFor(c => c.EndDate);
            }
        }
    }
}
