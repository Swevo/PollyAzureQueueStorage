public class PollyAzureQueueStorageServiceCollectionExtensionsTests
{
    private const string FakeConn = "DefaultEndpointsProtocol=https;AccountName=fake;AccountKey=AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==;EndpointSuffix=core.windows.net";
    private static readonly QueueClient _client = new(FakeConn, "test-queue");

    [Fact]
    public void AddPollyAzureQueueStorage_RegistersResiliencePipeline()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_client);
        services.AddPollyAzureQueueStorage(p => { });
        Assert.NotNull(services.BuildServiceProvider().GetRequiredService<ResiliencePipeline>());
    }

    [Fact]
    public void AddPollyAzureQueueStorage_RegistersResilientQueueClient()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_client);
        services.AddPollyAzureQueueStorage(p => { });
        var resilient = services.BuildServiceProvider().GetRequiredService<ResilientQueueClient>();
        Assert.NotNull(resilient);
        Assert.Same(_client, resilient.Inner);
    }

    [Fact]
    public void AddPollyAzureQueueStorage_WithConnectionString_RegistersClient()
    {
        var services = new ServiceCollection();
        services.AddPollyAzureQueueStorage(FakeConn, "test-queue", p => { });
        Assert.NotNull(services.BuildServiceProvider().GetRequiredService<ResilientQueueClient>());
    }

    [Fact]
    public void AddPollyAzureQueueStorage_ReturnsServiceCollection()
    {
        var services = new ServiceCollection();
        services.AddSingleton(_client);
        Assert.Same(services, services.AddPollyAzureQueueStorage(p => { }));
    }
}
