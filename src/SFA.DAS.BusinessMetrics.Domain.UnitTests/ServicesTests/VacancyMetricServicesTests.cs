using AutoFixture.NUnit3;
using Azure.Core;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.BusinessMetrics.Domain.Models;
using SFA.DAS.BusinessMetrics.Domain.Services;
using SFA.DAS.Testing.AutoFixture;


namespace SFA.DAS.BusinessMetrics.Domain.UnitTests.ServicesTests
{
    public class VacancyMetricServicesTests
    {
        [Test, MoqAutoData]
        public async Task GetVacancyMetrics_Returns_Count_When_Config_Found(
            DateTime startDate,
            DateTime endDate,
            List<VacancyMetrics> vacancyMetrics,
            LogAnalyticsWorkSpace logAnalyticsWorkSpace,
            MetricsConfiguration metricsConfiguration,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<ILogger<VacancyMetricServices>> mockLogger,
            [Frozen] Mock<IOptions<LogAnalyticsWorkSpace>> mockLogAnalyticsWorkspace,
            [Frozen] Mock<IOptions<MetricsConfiguration>> mockMetricsConfigurationOptions)
        {
            var logsTableColumns = new LogsTableColumn[]
            {
                MonitorQueryModelFactory.LogsTableColumn("CustomDimension", LogsColumnType.String),
                MonitorQueryModelFactory.LogsTableColumn("Name", LogsColumnType.String),
                MonitorQueryModelFactory.LogsTableColumn("Count", LogsColumnType.Long)
            };
            
            var logsTableRows = vacancyMetrics.Select(vacancyMetric => MonitorQueryModelFactory.LogsTableRow(logsTableColumns, [vacancyMetric.VacancyReference, vacancyMetric.Name, vacancyMetric.Count])).ToList();
            var logsTable = MonitorQueryModelFactory.LogsTable("tester", logsTableColumns.AsEnumerable(), logsTableRows.AsEnumerable());

            mockLogAnalyticsWorkspace.Setup(ap => ap.Value).Returns(logAnalyticsWorkSpace);
            mockMetricsConfigurationOptions.Setup(ap => ap.Value).Returns(metricsConfiguration);

            mockLogsQueryClient.Setup(x => x.ProcessQuery(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(),
                    It.IsAny<QueryTimeRange>(), CancellationToken.None))
                .ReturnsAsync(logsTable);

            var sut = new VacancyMetricServices(mockMetricsConfigurationOptions.Object, mockLogAnalyticsWorkspace.Object, mockLogsQueryClient.Object);

            var actual = await sut.GetVacancyMetrics(startDate, endDate, CancellationToken.None);

            actual.Count.Should().Be(vacancyMetrics.Count);
            actual.Should().BeEquivalentTo(vacancyMetrics);
        }
    }
}
