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
using SFA.DAS.BusinessMetrics.Domain.Services;
using SFA.DAS.Testing.AutoFixture;


namespace SFA.DAS.BusinessMetrics.Domain.UnitTests.ServicesTests
{
    public class VacancyMetricServicesTests
    {
        [Test, MoqAutoData]
        public async Task GetVacancyMetrics_Returns_Count_When_Config_Found(
            string vacancyReference,
            DateTime startDate,
            DateTime endDate,
            long viewsCount,
            string serviceName,
            string actionName,
            ServicesConfiguration servicesConfiguration,
            MetricsConfiguration metricsConfiguration,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<ILogger<VacancyMetricServices>> mockLogger,
            [Frozen] Mock<IOptions<ServicesConfiguration>> mockServicesConfigurationOptions,
            [Frozen] Mock<IOptions<MetricsConfiguration>> mockMetricsConfigurationOptions)
        {
            var logsTableColumns = new LogsTableColumn[]
            {
                MonitorQueryModelFactory.LogsTableColumn("sum_value", LogsColumnType.String)
            };
            var rowArray = new LogsTableRow[]
            {
                MonitorQueryModelFactory.LogsTableRow(logsTableColumns, [viewsCount])
            };
            var logsTable = MonitorQueryModelFactory.LogsTable("tester", logsTableColumns.AsEnumerable(), rowArray.AsEnumerable());

            servicesConfiguration.Resources.ForEach(r => r.ServiceName = serviceName);
            foreach (var customMetric in metricsConfiguration.CustomMetrics)
            {
                customMetric.ServiceName = serviceName;
                customMetric.Action = actionName;
            }

            mockServicesConfigurationOptions.Setup(ap => ap.Value).Returns(servicesConfiguration);
            mockMetricsConfigurationOptions.Setup(ap => ap.Value).Returns(metricsConfiguration);

            mockLogsQueryClient.Setup(x => x.ProcessQuery(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(),
                    It.IsAny<QueryTimeRange>(), CancellationToken.None))
                .ReturnsAsync(logsTable);

            var sut = new VacancyMetricServices(mockMetricsConfigurationOptions.Object, mockServicesConfigurationOptions.Object, mockLogsQueryClient.Object);

            var actual = await sut.GetVacancyMetrics(serviceName, actionName, vacancyReference, startDate, endDate, CancellationToken.None);

            actual.Should().Be(viewsCount);
        }

        [Test, MoqAutoData]
        public async Task GetVacancyMetrics_Returns_Zero_When_No_Results_Found(
            string vacancyReference,
            DateTime startDate,
            DateTime endDate,
            string serviceName,
            string actionName,
            ServicesConfiguration servicesConfiguration,
            MetricsConfiguration metricsConfiguration,
            [Frozen] Mock<ILogsQueryClient> mockLogsQueryClient,
            [Frozen] Mock<ILogger<VacancyMetricServices>> mockLogger,
            [Frozen] Mock<IOptions<ServicesConfiguration>> mockServicesConfigurationOptions,
            [Frozen] Mock<IOptions<MetricsConfiguration>> mockMetricsConfigurationOptions)
        {
            var logsTableColumns = new LogsTableColumn[]
            {
                MonitorQueryModelFactory.LogsTableColumn("sum_value", LogsColumnType.String)
            };
            var rowArray = new LogsTableRow[]
            {
                MonitorQueryModelFactory.LogsTableRow(logsTableColumns, [0])
            };
            var logsTable = MonitorQueryModelFactory.LogsTable("tester", logsTableColumns.AsEnumerable(), rowArray.AsEnumerable());

            mockLogsQueryClient.Setup(x => x.ProcessQuery(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(),
                    It.IsAny<QueryTimeRange>(), CancellationToken.None))
                .ReturnsAsync(logsTable);

            servicesConfiguration.Resources.ForEach(r => r.ServiceName = serviceName);
            foreach (var customMetric in metricsConfiguration.CustomMetrics)
            {
                customMetric.ServiceName = serviceName;
                customMetric.Action = actionName;
            }
            mockServicesConfigurationOptions.Setup(ap => ap.Value).Returns(servicesConfiguration);
            mockMetricsConfigurationOptions.Setup(ap => ap.Value).Returns(metricsConfiguration);

            var sut = new VacancyMetricServices(mockMetricsConfigurationOptions.Object, mockServicesConfigurationOptions.Object, mockLogsQueryClient.Object);

            var actual = await sut.GetVacancyMetrics(serviceName, actionName, vacancyReference, startDate, endDate, CancellationToken.None);
            
            actual.Should().Be(0);
        }
    }
}
