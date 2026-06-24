public class PollyAzureQueueStorageExtensionsTests
{
    private const string FakeConn = "DefaultEndpointsProtocol=https;AccountName=fake;AccountKey=AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==;EndpointSuffix=core.windows.net";
    private static readonly QueueClient _client = new(FakeConn, "test-queue");
    private static readonly ResiliencePipeline _pipeline = new ResiliencePipelineBuilder().Build();

    [Fact]
    public void WithPolly_Pipeline_ReturnsResilientQueueClient()
    {
        var resilient = _client.WithPolly(_pipeline);
        Assert.NotNull(resilient);
        Assert.Same(_client, resilient.Inner);
    }

    [Fact]
    public void WithPolly_Configure_ReturnsResilientQueueClient()
    {
        var resilient = _client.WithPolly(p => p.AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3, Delay = TimeSpan.Zero,
            ShouldHandle = QueueStorageTransientErrors.IsTransient,
        }));
        Assert.NotNull(resilient);
        Assert.Same(_client, resilient.Inner);
    }
}
