/// <summary>Extension methods for adding Polly resilience to Azure Queue Storage clients.</summary>
public static class PollyAzureQueueStorageExtensions
{
    /// <summary>Wraps a <see cref="QueueClient"/> with the given <see cref="ResiliencePipeline"/>.</summary>
    public static ResilientQueueClient WithPolly(
        this QueueClient client,
        ResiliencePipeline pipeline)
        => new(client, pipeline);

    /// <summary>Wraps a <see cref="QueueClient"/> with a pipeline built by <paramref name="configure"/>.</summary>
    public static ResilientQueueClient WithPolly(
        this QueueClient client,
        Action<ResiliencePipelineBuilder> configure)
    {
        var builder = new ResiliencePipelineBuilder();
        configure(builder);
        return new(client, builder.Build());
    }
}
