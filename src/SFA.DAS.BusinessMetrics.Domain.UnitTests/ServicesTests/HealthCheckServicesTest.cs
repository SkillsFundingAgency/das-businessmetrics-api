using AutoFixture.NUnit3;
using Azure.Core;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using FluentAssertions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.BusinessMetrics.Domain.Services;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.BusinessMetrics.Domain.UnitTests.ServicesTests
{
    public class HealthCheckServicesTest
    {
        [Test, MoqAutoData]
        public async Task GetHealthCheck_Returns_HealthCheckResult(
            List<string> computerNames,
            LogAnalyticsWorkSpace logAnalyticsWorkSpace,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<ILogger<VacancyMetricServices>> mockLogger,
            [Frozen] Mock<IOptions<LogAnalyticsWorkSpace>> mockLogAnalyticsWorkspace)
        {
            var logsTableColumns = new LogsTableColumn[]
            {
                MonitorQueryModelFactory.LogsTableColumn("Computer", LogsColumnType.String),
            };

            var logsTableRows = computerNames.Select(name => MonitorQueryModelFactory.LogsTableRow(logsTableColumns, [name])).ToList();
            var logsTable = MonitorQueryModelFactory.LogsTable("tester", logsTableColumns.AsEnumerable(), logsTableRows.AsEnumerable());

            mockLogAnalyticsWorkspace.Setup(ap => ap.Value).Returns(logAnalyticsWorkSpace);
            mockLogsQueryClient.Setup(x => x.ProcessQuery(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(),
                    It.IsAny<QueryTimeRange>(), CancellationToken.None))
                .ReturnsAsync(logsTable);

            var sut = new HealthCheckServices(mockLogAnalyticsWorkspace.Object, mockLogsQueryClient.Object);

            var actual = await sut.GetHealthCheck(CancellationToken.None);

            actual.Should().Be(HealthCheckResult.Healthy());
        }

        [Test, MoqAutoData]
        public async Task Then_GetHealthCheck_Returns_DegradedHealthCheckResult(
            List<string> computerNames,
            LogAnalyticsWorkSpace logAnalyticsWorkSpace,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<ILogger<VacancyMetricServices>> mockLogger,
            [Frozen] Mock<IOptions<LogAnalyticsWorkSpace>> mockLogAnalyticsWorkspace)
        {
            computerNames = [];
            var logsTableColumns = new LogsTableColumn[]
            {
                MonitorQueryModelFactory.LogsTableColumn("Computer", LogsColumnType.String),
            };

            var logsTableRows = computerNames.Select(name => MonitorQueryModelFactory.LogsTableRow(logsTableColumns, [name])).ToList();
            var logsTable = MonitorQueryModelFactory.LogsTable("tester", logsTableColumns.AsEnumerable(), logsTableRows.AsEnumerable());

            mockLogAnalyticsWorkspace.Setup(ap => ap.Value).Returns(logAnalyticsWorkSpace);
            mockLogsQueryClient.Setup(x => x.ProcessQuery(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(),
                    It.IsAny<QueryTimeRange>(), CancellationToken.None))
                .ReturnsAsync(logsTable);

            var sut = new HealthCheckServices(mockLogAnalyticsWorkspace.Object, mockLogsQueryClient.Object);

            var actual = await sut.GetHealthCheck(CancellationToken.None);

            actual.Should().Be(HealthCheckResult.Degraded());
        }
    }
}
