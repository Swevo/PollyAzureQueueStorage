/// <summary>
/// Wraps a <see cref="QueueClient"/> with a Polly v8 <see cref="ResiliencePipeline"/>,
/// applying retry, timeout, and circuit-breaker to every queue operation.
/// </summary>
public sealed class ResilientQueueClient(QueueClient client, ResiliencePipeline pipeline)
{
    /// <summary>The underlying <see cref="QueueClient"/>.</summary>
    public QueueClient Inner => client;

    /// <summary>Sends a message to the queue, protected by the resilience pipeline.</summary>
    public Task<Response<SendReceipt>> SendMessageAsync(
        string messageText,
        CancellationToken cancellationToken = default)
        => pipeline.ExecuteAsync(
            async ct => await client.SendMessageAsync(messageText, ct),
            cancellationToken).AsTask();

    /// <summary>Sends a message with visibility and TTL options, protected by the resilience pipeline.</summary>
    public Task<Response<SendReceipt>> SendMessageAsync(
        string messageText,
        TimeSpan? visibilityTimeout = null,
        TimeSpan? timeToLive = null,
        CancellationToken cancellationToken = default)
        => pipeline.ExecuteAsync(
            async ct => await client.SendMessageAsync(messageText, visibilityTimeout, timeToLive, ct),
            cancellationToken).AsTask();

    /// <summary>Receives messages from the queue, protected by the resilience pipeline.</summary>
    public Task<Response<QueueMessage[]>> ReceiveMessagesAsync(
        int? maxMessages = null,
        CancellationToken cancellationToken = default)
        => pipeline.ExecuteAsync(
            async ct => await client.ReceiveMessagesAsync(maxMessages, cancellationToken: ct),
            cancellationToken).AsTask();

    /// <summary>Peeks at messages without dequeuing, protected by the resilience pipeline.</summary>
    public Task<Response<PeekedMessage[]>> PeekMessagesAsync(
        int? maxMessages = null,
        CancellationToken cancellationToken = default)
        => pipeline.ExecuteAsync(
            async ct => await client.PeekMessagesAsync(maxMessages, ct),
            cancellationToken).AsTask();

    /// <summary>Deletes a message from the queue, protected by the resilience pipeline.</summary>
    public Task<Response> DeleteMessageAsync(
        string messageId,
        string popReceipt,
        CancellationToken cancellationToken = default)
        => pipeline.ExecuteAsync(
            async ct => await client.DeleteMessageAsync(messageId, popReceipt, ct),
            cancellationToken).AsTask();

    /// <summary>
    /// Executes any <see cref="QueueClient"/> operation, protected by the resilience pipeline.
    /// </summary>
    public Task<T> ExecuteAsync<T>(
        Func<QueueClient, CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
        => pipeline.ExecuteAsync(
            async ct => await operation(client, ct),
            cancellationToken).AsTask();
}
