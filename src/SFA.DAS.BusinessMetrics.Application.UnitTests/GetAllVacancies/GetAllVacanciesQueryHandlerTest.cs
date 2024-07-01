using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.BusinessMetrics.Application.GetAllVacancies.Queries;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.BusinessMetrics.Application.UnitTests.GetAllVacancies
{
    public class GetAllVacanciesQueryHandlerTest
    {
        [Test, RecursiveMoqAutoData]
        public async Task Handle_Returns_All_Vacancies(
            [Frozen] Mock<IVacancyMetricServices> metricServices,
            GetAllVacanciesQueryHandler sut,
            GetAllVacanciesQuery request,
            List<string> vacancies)
        {
            metricServices
                .Setup(a => a.GetAllVacancies(request.StartDate, request.EndDate, CancellationToken.None))!
                .ReturnsAsync(vacancies);
           
            var response = await sut.Handle(request, new CancellationToken());

            response.Result.Vacancies.Should().BeEquivalentTo(vacancies);
        }
    }
}
