using AutoFixture.NUnit3;
using Azure.Core;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.BusinessMetrics.Domain.Models;
using SFA.DAS.BusinessMetrics.Domain.Services;
using SFA.DAS.Testing.AutoFixture;
using System.Configuration;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;

namespace SFA.DAS.BusinessMetrics.Domain.UnitTests.ServicesTests
{
    public class VacancyMetricServicesTests
    {
        [Test, MoqAutoData]
        public void GetMetricServiceNames_Returns_ServiceNames(
            List<VacancyViewsConfig> config,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<ILogger<VacancyMetricServices>> mockLogger,
            [Frozen] Mock<IOptions<MetricsConfiguration>> mockOptions)
        {
            var sut = new VacancyMetricServices(mockLogger.Object, mockOptions.Object,
                mockLogsQueryClient.Object);

            var actual = sut.GetMetricServiceNames();

            actual.Should().NotBeNull();
            actual.Count.Should().Be(config.Count);
        }

        [Test, MoqAutoData]
        public void GetVacancyMetrics_Returns_Exception_When_Config_NotFound(
            string serviceName,
            string vacancyReference,
            DateTime startDate,
            DateTime endDate,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<ILogger<VacancyMetricServices>> mockLogger,
            [Frozen] Mock<IOptions<MetricsConfiguration>> mockOptions)
        {
            var sut = new VacancyMetricServices(mockLogger.Object, mockOptions.Object,
                mockLogsQueryClient.Object);

            Assert.ThrowsAsync<ConfigurationErrorsException>(() => sut.GetVacancyMetrics(serviceName, vacancyReference, startDate, endDate, CancellationToken.None));

            mockLogger.Verify(l =>
                l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, type) => state.ToString()!.Contains("Configuration could not be found for service name")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()!
                ), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task GetVacancyMetrics_Returns_Count_When_Config_Found(
            string vacancyReference,
            DateTime startDate,
            DateTime endDate,
            long viewsCount,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<ILogger<VacancyMetricServices>> mockLogger,
            [Frozen] Mock<IOptions<MetricsConfiguration>> mockOptions)
        {
            var logsTableColumns = new LogsTableColumn[]
            {
                MonitorQueryModelFactory.LogsTableColumn("sum_value", LogsColumnType.String)
            };
            var rowArray = new LogsTableRow[]
            {
                MonitorQueryModelFactory.LogsTableRow(logsTableColumns, new object[] { viewsCount })
            };
            var logsTable = MonitorQueryModelFactory.LogsTable("tester", logsTableColumns.AsEnumerable(), rowArray.AsEnumerable());

            var config = mockOptions.Object.Value.VacancyViewsConfig.FirstOrDefault();

            mockLogsQueryClient.Setup(x => x.ProcessQuery(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(),
                    It.IsAny<QueryTimeRange>(), CancellationToken.None))
                .ReturnsAsync(logsTable);

            var sut = new VacancyMetricServices(mockLogger.Object, mockOptions.Object,
                mockLogsQueryClient.Object);

            var actual = await sut.GetVacancyMetrics(config!.ServiceName, vacancyReference, startDate, endDate, CancellationToken.None);

            actual.Should().Be(viewsCount);
        }
    }
}
