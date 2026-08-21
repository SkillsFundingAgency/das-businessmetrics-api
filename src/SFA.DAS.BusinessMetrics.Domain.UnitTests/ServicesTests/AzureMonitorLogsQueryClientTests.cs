using Azure;
using Azure.Core;
using Azure.Monitor.Query;
using SFA.DAS.BusinessMetrics.Domain.Services;

namespace SFA.DAS.BusinessMetrics.Domain.UnitTests.ServicesTests;

[TestFixture]
internal class AzureMonitorLogsQueryClientTests
{
    [Test, MoqAutoData]
    public async Task ProcessQuery_WhenClientThrows_PropagatesException(
        ResourceIdentifier resourceIdentifier,
        string query,
        QueryTimeRange timeRange)
    {
        var mockClient = Mock.Of<LogsQueryClient>();
        Mock.Get(mockClient)
            .Setup(x => x.QueryResourceAsync(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(), It.IsAny<QueryTimeRange>(), null, CancellationToken.None))
            .ThrowsAsync(new RequestFailedException("Service unavailable"));

        var sut = new AzureMonitorLogsQueryClient(mockClient);
        var act = async () => await sut.ProcessQuery(resourceIdentifier, query, timeRange, CancellationToken.None);

        await act.Should().ThrowAsync<RequestFailedException>().WithMessage("Service unavailable");
    }

    [Test, MoqAutoData]
    public async Task ProcessQuery_WhenCancelled_PropagatesCancellation(
        ResourceIdentifier resourceIdentifier,
        string query,
        QueryTimeRange timeRange)
    {
        var mockClient = Mock.Of<LogsQueryClient>();
        Mock.Get(mockClient)
            .Setup(x => x.QueryResourceAsync(It.IsAny<ResourceIdentifier>(), It.IsAny<string>(), It.IsAny<QueryTimeRange>(), null, CancellationToken.None))
            .ThrowsAsync(new OperationCanceledException());

        var sut = new AzureMonitorLogsQueryClient(mockClient);
        var act = async () => await sut.ProcessQuery(resourceIdentifier, query, timeRange, CancellationToken.None);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}