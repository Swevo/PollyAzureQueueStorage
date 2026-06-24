# PollyAzureQueueStorage

[![NuGet](https://img.shields.io/nuget/v/PollyAzureQueueStorage.svg)](https://www.nuget.org/packages/PollyAzureQueueStorage/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/PollyAzureQueueStorage.svg)](https://www.nuget.org/packages/PollyAzureQueueStorage/)
[![CI](https://github.com/Swevo/PollyAzureQueueStorage/actions/workflows/build.yml/badge.svg)](https://github.com/Swevo/PollyAzureQueueStorage/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

**Polly v8 resilience pipelines for Azure Queue Storage** — retry, timeout, and circuit-breaker for QueueClient operations.

## Why PollyAzureQueueStorage?

Azure Queue Storage is a durable, scalable message queue — but transient failures (throttling 429, service unavailability 503, gateway timeouts 504) can disrupt your messaging pipeline. PollyAzureQueueStorage wraps QueueClient in a Polly v8 ResiliencePipeline so send, receive, peek, and delete all automatically retry on transient errors.

Completes the **Azure storage trio** alongside [PollyAzureBlob](https://github.com/Swevo/PollyAzureBlob) and [PollyAzureTableStorage](https://github.com/Swevo/PollyAzureTableStorage).

## Installation

`
dotnet add package PollyAzureQueueStorage
`

## Quick start

### DI registration

`csharp
services.AddPollyAzureQueueStorage(
    connectionString: Environment.GetEnvironmentVariable("AZURE_STORAGE_CONN"),
    queueName: "my-queue",
    configure: pipeline => pipeline
        .AddRetry(new RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(1),
            BackoffType = DelayBackoffType.Exponential,
            ShouldHandle = QueueStorageTransientErrors.IsTransient,
        })
        .AddTimeout(TimeSpan.FromSeconds(30)));

// Use via DI
public class QueueService(ResilientQueueClient queue)
{
    public async Task SendAsync(string message, CancellationToken ct = default)
        => await queue.SendMessageAsync(message, ct);
}
`

### WithPolly() extension on existing QueueClient

`csharp
var queueClient = new QueueClient(connectionString, "my-queue");

var resilient = queueClient.WithPolly(pipeline =>
    pipeline.AddRetry(new RetryStrategyOptions
    {
        MaxRetryAttempts = 3,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential,
        ShouldHandle = QueueStorageTransientErrors.IsTransient,
    }));

await resilient.SendMessageAsync("hello");
var messages = await resilient.ReceiveMessagesAsync(maxMessages: 10);
`

## API surface

| Method | Description |
|---|---|
| SendMessageAsync(string, CancellationToken) | Enqueue a plain-text message |
| SendMessageAsync(BinaryData, CancellationToken) | Enqueue binary/JSON message |
| ReceiveMessagesAsync(int, TimeSpan?, CancellationToken) | Dequeue up to N messages |
| PeekMessagesAsync(int, CancellationToken) | Peek without dequeuing |
| DeleteMessageAsync(string, string, CancellationToken) | Delete after processing |
| ExecuteAsync<T>(Func<CancellationToken, ValueTask<T>>, CancellationToken) | Custom operation |

## Transient error predicate

QueueStorageTransientErrors.IsTransient handles:

| Exception | Condition |
|---|---|
| RequestFailedException | Status 429 (throttled), 503 (unavailable), 504 (gateway timeout) |
| HttpRequestException | Network-level failure |
| TaskCanceledException | Timeout or cancellation |

## Supported frameworks


et6.0 · 
et8.0 · 
et9.0

## Related packages

| Package | Wraps |
|---|---|
| [PollyEFCore](https://github.com/Swevo/PollyEFCore) | Entity Framework Core DbContext |
| [PollyDapper](https://github.com/Swevo/PollyDapper) | Dapper IDbConnection |
| [PollyMongo](https://github.com/Swevo/PollyMongo) | MongoDB IMongoCollection<T> |
| [PollyAzureBlob](https://github.com/Swevo/PollyAzureBlob) | Azure Blob Storage BlobContainerClient |
| [PollyNpgsql](https://github.com/Swevo/PollyNpgsql) | Npgsql PostgreSQL NpgsqlConnection |
| [PollySqlClient](https://github.com/Swevo/PollySqlClient) | System.Data.SqlClient SqlConnection |
| [PollyCosmosDb](https://github.com/Swevo/PollyCosmosDb) | Azure Cosmos DB CosmosClient |
| [PollyGrpc](https://github.com/Swevo/PollyGrpc) | gRPC channel calls |
| [PollyRabbitMQ](https://github.com/Swevo/PollyRabbitMQ) | RabbitMQ IModel channel |
| [PollyAzureServiceBus](https://github.com/Swevo/PollyAzureServiceBus) | Azure Service Bus sender/receiver |
| [PollyRedis](https://github.com/Swevo/PollyRedis) | StackExchange.Redis IDatabase |
| [PollyMediatR](https://github.com/Swevo/PollyMediatR) | MediatR IMediator |
| [PollyOpenAI](https://github.com/Swevo/PollyOpenAI) | OpenAI ChatClient |
| [PollyHealthChecks](https://github.com/Swevo/PollyHealthChecks) | ASP.NET Core health checks |
| [PollyBackoff](https://github.com/Swevo/PollyBackoff) | Pre-built backoff pipelines |
| [PollyChaos](https://github.com/Swevo/PollyChaos) | Chaos engineering helpers |
| [PollyKafka](https://github.com/Swevo/PollyKafka) | Confluent Kafka producer/consumer |
| [PollySignalR](https://github.com/Swevo/PollySignalR) | SignalR HubConnection |
| [PollyRateLimiter](https://github.com/Swevo/PollyRateLimiter) | .NET rate limiting middleware |
| [PollyElasticsearch](https://github.com/Swevo/PollyElasticsearch) | Elastic.Clients.Elasticsearch |
| [PollyAzureKeyVault](https://github.com/Swevo/PollyAzureKeyVault) | Azure Key Vault SecretClient |
| [PollyAzureEventHub](https://github.com/Swevo/PollyAzureEventHub) | Azure Event Hubs producer |
| [PollySendGrid](https://github.com/Swevo/PollySendGrid) | SendGrid email client |
| [PollyMassTransit](https://github.com/Swevo/PollyMassTransit) | MassTransit IBus |
| [PollyAzureTableStorage](https://github.com/Swevo/PollyAzureTableStorage) | Azure Table Storage TableClient |
| [PollyMailKit](https://github.com/Swevo/PollyMailKit) | MailKit SMTP email client |
| [PollyHangfire](https://github.com/Swevo/PollyHangfire) | Hangfire IBackgroundJobClient |

## License

MIT © Justin Bannister