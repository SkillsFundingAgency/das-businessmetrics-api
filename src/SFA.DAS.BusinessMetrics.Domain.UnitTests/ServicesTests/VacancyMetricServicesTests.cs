using Azure.Core;
using Azure.Monitor.Query;
using Azure.Monitor.Query.Models;
using Microsoft.Extensions.Options;
using SFA.DAS.BusinessMetrics.Domain.Configuration;
using SFA.DAS.BusinessMetrics.Domain.Interfaces.Services;
using SFA.DAS.BusinessMetrics.Domain.Models;
using SFA.DAS.BusinessMetrics.Domain.Services;

namespace SFA.DAS.BusinessMetrics.Domain.UnitTests.ServicesTests
{
    public class VacancyMetricServicesTests
    {
        private static LogsTable BuildLogsTable(IEnumerable<VacancyMetrics> vacancyMetrics)
        {
            var columns = new LogsTableColumn[]
            {
                MonitorQueryModelFactory.LogsTableColumn("CustomDimension", LogsColumnType.String),
                MonitorQueryModelFactory.LogsTableColumn("Name", LogsColumnType.String),
                MonitorQueryModelFactory.LogsTableColumn("Count", LogsColumnType.Long)
            };

            var rows = vacancyMetrics
                .Select(m => MonitorQueryModelFactory.LogsTableRow(columns, [m.VacancyReference, m.Name, m.Count]))
                .ToList();

            return MonitorQueryModelFactory.LogsTable("tester", columns.AsEnumerable(), rows.AsEnumerable());
        }

        private static VacancyMetricServices BuildSut(
            Mock<ILogsQueryClient> mockLogsQueryClient,
            Mock<IOptions<LogAnalyticsWorkSpace>> mockLogAnalyticsWorkspace)
            => new(mockLogAnalyticsWorkspace.Object, mockLogsQueryClient.Object);

        [Test, MoqAutoData]
        public async Task GetVacancyMetrics_Returns_Count_When_Config_Found(
            DateTime startDate,
            DateTime endDate,
            List<VacancyMetrics> vacancyMetrics,
            LogAnalyticsWorkSpace logAnalyticsWorkSpace,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
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

            mockLogsQueryClient.Setup(x => x.ProcessQuery(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(),
                    It.IsAny<QueryTimeRange>(), CancellationToken.None))
                .ReturnsAsync(logsTable);

            var sut = new VacancyMetricServices(mockLogAnalyticsWorkspace.Object, mockLogsQueryClient.Object);

            var actual = await sut.GetVacancyMetrics(startDate, endDate, CancellationToken.None);

            actual.Count.Should().Be(vacancyMetrics.Count);
            actual.Should().BeEquivalentTo(vacancyMetrics);
        }

        [Test, MoqAutoData]
        public async Task GetVacancyMetrics_WhenRowsReturned_MapsAllFieldsCorrectly(
            DateTime startDate,
            DateTime endDate,
            List<VacancyMetrics> vacancyMetrics,
            LogAnalyticsWorkSpace logAnalyticsWorkSpace,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<IOptions<LogAnalyticsWorkSpace>> mockLogAnalyticsWorkspace)
        {
            mockLogAnalyticsWorkspace.Setup(x => x.Value).Returns(logAnalyticsWorkSpace);
            mockLogsQueryClient
                .Setup(x => x.ProcessQuery(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(), It.IsAny<QueryTimeRange>(), CancellationToken.None))
                .ReturnsAsync(BuildLogsTable(vacancyMetrics));

            var actual = await BuildSut(mockLogsQueryClient, mockLogAnalyticsWorkspace)
                .GetVacancyMetrics(startDate, endDate, CancellationToken.None);

            actual.Should().BeEquivalentTo(vacancyMetrics);
        }

        [Test, MoqAutoData]
        public async Task GetVacancyMetrics_WhenRowsReturned_ReturnsCorrectCount(
            DateTime startDate,
            DateTime endDate,
            List<VacancyMetrics> vacancyMetrics,
            LogAnalyticsWorkSpace logAnalyticsWorkSpace,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<IOptions<LogAnalyticsWorkSpace>> mockLogAnalyticsWorkspace)
        {
            mockLogAnalyticsWorkspace.Setup(x => x.Value).Returns(logAnalyticsWorkSpace);
            mockLogsQueryClient
                .Setup(x => x.ProcessQuery(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(), It.IsAny<QueryTimeRange>(), CancellationToken.None))
                .ReturnsAsync(BuildLogsTable(vacancyMetrics));

            var actual = await BuildSut(mockLogsQueryClient, mockLogAnalyticsWorkspace)
                .GetVacancyMetrics(startDate, endDate, CancellationToken.None);

            actual.Count.Should().Be(vacancyMetrics.Count);
        }

        [Test, MoqAutoData]
        public async Task GetVacancyMetrics_WhenTableIsNull_ReturnsEmptyList(
            DateTime startDate,
            DateTime endDate,
            LogAnalyticsWorkSpace logAnalyticsWorkSpace,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<IOptions<LogAnalyticsWorkSpace>> mockLogAnalyticsWorkspace)
        {
            mockLogAnalyticsWorkspace.Setup(x => x.Value).Returns(logAnalyticsWorkSpace);
            mockLogsQueryClient
                .Setup(x => x.ProcessQuery(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(), It.IsAny<QueryTimeRange>(), CancellationToken.None))
                .ReturnsAsync((LogsTable)null!);

            var actual = await BuildSut(mockLogsQueryClient, mockLogAnalyticsWorkspace)
                .GetVacancyMetrics(startDate, endDate, CancellationToken.None);

            actual.Should().BeEmpty();
        }

        [Test, MoqAutoData]
        public async Task GetVacancyMetrics_WhenTableHasNoRows_ReturnsEmptyList(
            DateTime startDate,
            DateTime endDate,
            LogAnalyticsWorkSpace logAnalyticsWorkSpace,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<IOptions<LogAnalyticsWorkSpace>> mockLogAnalyticsWorkspace)
        {
            var emptyTable = BuildLogsTable([]);

            mockLogAnalyticsWorkspace.Setup(x => x.Value).Returns(logAnalyticsWorkSpace);
            mockLogsQueryClient
                .Setup(x => x.ProcessQuery(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(), It.IsAny<QueryTimeRange>(), CancellationToken.None))
                .ReturnsAsync(emptyTable);

            var actual = await BuildSut(mockLogsQueryClient, mockLogAnalyticsWorkspace)
                .GetVacancyMetrics(startDate, endDate, CancellationToken.None);

            actual.Should().BeEmpty();
        }

        [Test, MoqAutoData]
        public async Task GetVacancyMetrics_PassesCorrectDateRangeToQueryClient(
            DateTime startDate,
            DateTime endDate,
            LogAnalyticsWorkSpace logAnalyticsWorkSpace,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<IOptions<LogAnalyticsWorkSpace>> mockLogAnalyticsWorkspace)
        {
            mockLogAnalyticsWorkspace.Setup(x => x.Value).Returns(logAnalyticsWorkSpace);
            mockLogsQueryClient
                .Setup(x => x.ProcessQuery(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(), It.IsAny<QueryTimeRange>(), CancellationToken.None))
                .ReturnsAsync(BuildLogsTable([]));

            await BuildSut(mockLogsQueryClient, mockLogAnalyticsWorkspace)
                .GetVacancyMetrics(startDate, endDate, CancellationToken.None);

            mockLogsQueryClient.Verify(x => x.ProcessQuery(
                It.IsAny<ResourceIdentifier>(),
                It.IsAny<string>(),
                It.Is<QueryTimeRange>(r => r.Start == startDate && r.End == endDate),
                CancellationToken.None), Times.Once);
        }

        [Test, MoqAutoData]
        public async Task GetVacancyMetrics_PassesCorrectResourceIdentifierToQueryClient(
            DateTime startDate,
            DateTime endDate,
            LogAnalyticsWorkSpace logAnalyticsWorkSpace,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<IOptions<LogAnalyticsWorkSpace>> mockLogAnalyticsWorkspace)
        {
            mockLogAnalyticsWorkspace.Setup(x => x.Value).Returns(logAnalyticsWorkSpace);
            mockLogsQueryClient
                .Setup(x => x.ProcessQuery(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(), It.IsAny<QueryTimeRange>(), CancellationToken.None))
                .ReturnsAsync(BuildLogsTable([]));

            await BuildSut(mockLogsQueryClient, mockLogAnalyticsWorkspace)
                .GetVacancyMetrics(startDate, endDate, CancellationToken.None);

            mockLogsQueryClient.Verify(x => x.ProcessQuery(
                It.Is<ResourceIdentifier>(r => r.ToString() == logAnalyticsWorkSpace.Identifier),
                It.IsAny<string>(),
                It.IsAny<QueryTimeRange>(),
                CancellationToken.None), Times.Once);
        }
    }
}