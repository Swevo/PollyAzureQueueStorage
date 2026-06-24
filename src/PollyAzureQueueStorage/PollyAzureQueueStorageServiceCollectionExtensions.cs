/// <summary>Dependency-injection extensions for <c>PollyAzureQueueStorage</c>.</summary>
public static class PollyAzureQueueStorageServiceCollectionExtensions
{
    /// <summary>
    /// Registers a singleton <see cref="ResiliencePipeline"/> and a transient
    /// <see cref="ResilientQueueClient"/> wrapping the <see cref="QueueClient"/>
    /// already registered in the DI container.
    /// </summary>
    public static IServiceCollection AddPollyAzureQueueStorage(
        this IServiceCollection services,
        Action<ResiliencePipelineBuilder> configure)
    {
        var builder = new ResiliencePipelineBuilder();
        configure(builder);
        var pipeline = builder.Build();

        services.AddSingleton(pipeline);
        services.AddTransient<ResilientQueueClient>(sp =>
            sp.GetRequiredService<QueueClient>().WithPolly(pipeline));

        return services;
    }

    /// <summary>
    /// Registers a singleton <see cref="QueueClient"/> for the given connection string and queue name,
    /// then registers the resilience pipeline and <see cref="ResilientQueueClient"/>.
    /// </summary>
    public static IServiceCollection AddPollyAzureQueueStorage(
        this IServiceCollection services,
        string connectionString,
        string queueName,
        Action<ResiliencePipelineBuilder> configure)
    {
        services.AddSingleton(new QueueClient(connectionString, queueName));
        return services.AddPollyAzureQueueStorage(configure);
    }
}
