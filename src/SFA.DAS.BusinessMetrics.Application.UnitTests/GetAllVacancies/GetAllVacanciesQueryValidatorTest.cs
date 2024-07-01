using FluentValidation.TestHelper;
using SFA.DAS.BusinessMetrics.Application.GetAllVacancies.Queries;

namespace SFA.DAS.BusinessMetrics.Application.UnitTests.GetAllVacancies
{
    public class GetAllVacanciesQueryValidatorTest
    {
        [TestCase("1 jan 2024", "10 jan 2024", true)]
        [TestCase("1 jan 2024", "31 dec 2023", false)]
        public async Task Validates_ServiceName_NotNull_Not_Empty(string startDate, string endDate, bool isValid)
        {
            var query = new GetAllVacanciesQuery(Convert.ToDateTime(startDate), Convert.ToDateTime(endDate));
            var sut = new GetAllVacanciesQueryValidator();
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
