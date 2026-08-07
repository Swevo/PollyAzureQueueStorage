# PollyAzureQueueStorage

[![NuGet](https://img.shields.io/nuget/v/PollyAzureQueueStorage.svg)](https://www.nuget.org/packages/PollyAzureQueueStorage/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/PollyAzureQueueStorage.svg)](https://www.nuget.org/packages/PollyAzureQueueStorage/)
[![CI](https://github.com/Swevo/PollyAzureQueueStorage/actions/workflows/build.yml/badge.svg)](https://github.com/Swevo/PollyAzureQueueStorage/actions/workflows/build.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET 10 Ready](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](#)

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

## Related Packages

| Package | Downloads | Description |
|---|---|---|
| [PollyHealthChecks](https://www.nuget.org/packages/PollyHealthChecks) | [![Downloads](https://img.shields.io/nuget/dt/PollyHealthChecks.svg)](https://www.nuget.org/packages/PollyHealthChecks) | ASP.NET Core health checks for Polly v8 circuit breakers — expose circuit-breaker state (Closed, HalfOpen, Open, Isolated) as /health endpoint responses |
| [PollyBackoff](https://www.nuget.org/packages/PollyBackoff) | [![Downloads](https://img.shields.io/nuget/dt/PollyBackoff.svg)](https://www.nuget.org/packages/PollyBackoff) | Backoff delay strategies for Polly v8 resilience pipelines |
| [PollyGrpc](https://www.nuget.org/packages/PollyGrpc) | [![Downloads](https://img.shields.io/nuget/dt/PollyGrpc.svg)](https://www.nuget.org/packages/PollyGrpc) | Polly v8 resilience interceptor for gRPC |
| [PollyEFCore](https://www.nuget.org/packages/PollyEFCore) | [![Downloads](https://img.shields.io/nuget/dt/PollyEFCore.svg)](https://www.nuget.org/packages/PollyEFCore) | Polly v8 resilience pipelines for Entity Framework Core — wrap every EF Core query and SaveChanges with retry, timeout and circuit-breaker via a single AddPollyResilience() call |
| [PollyRabbitMQ](https://www.nuget.org/packages/PollyRabbitMQ) | [![Downloads](https://img.shields.io/nuget/dt/PollyRabbitMQ.svg)](https://www.nuget.org/packages/PollyRabbitMQ) | Polly v8 resilience for RabbitMQ.Client v7+ — retry, circuit-breaker, and timeout for IChannel operations, with built-in RabbitMqTransientErrors predicate covering AlreadyClosedException, BrokerUnreachableException, OperationInterruptedException, and ConnectFailureException |
| [PollyMailKit](https://www.nuget.org/packages/PollyMailKit) | [![Downloads](https://img.shields.io/nuget/dt/PollyMailKit.svg)](https://www.nuget.org/packages/PollyMailKit) | Polly v8 resilience pipelines for MailKit — retry, timeout, and circuit-breaker for SmtpClient.SendAsync and any MailKit SMTP operation |
| [PollyMassTransit](https://www.nuget.org/packages/PollyMassTransit) | [![Downloads](https://img.shields.io/nuget/dt/PollyMassTransit.svg)](https://www.nuget.org/packages/PollyMassTransit) | Polly v8 resilience pipelines for MassTransit — retry, timeout, and circuit-breaker for IBus.Publish and ISendEndpointProvider.Send |
| [PollyNpgsql](https://www.nuget.org/packages/PollyNpgsql) | [![Downloads](https://img.shields.io/nuget/dt/PollyNpgsql.svg)](https://www.nuget.org/packages/PollyNpgsql) | Polly v8 resilience pipelines for Npgsql (PostgreSQL) — retry, timeout, and circuit-breaker for NpgsqlConnection queries and commands, plus a built-in PostgresTransientErrors predicate covering all common PostgreSQL transient SQLSTATE codes |
| [PollyOpenAI](https://www.nuget.org/packages/PollyOpenAI) | [![Downloads](https://img.shields.io/nuget/dt/PollyOpenAI.svg)](https://www.nuget.org/packages/PollyOpenAI) | Polly v8 resilience for OpenAI and Azure OpenAI API calls |
| [PollyAzureEventHub](https://www.nuget.org/packages/PollyAzureEventHub) | [![Downloads](https://img.shields.io/nuget/dt/PollyAzureEventHub.svg)](https://www.nuget.org/packages/PollyAzureEventHub) | Polly v8 resilience pipelines for Azure Event Hubs — retry, timeout, and circuit-breaker for EventHubProducerClient and EventHubConsumerClient |
| [PollySignalR](https://www.nuget.org/packages/PollySignalR) | [![Downloads](https://img.shields.io/nuget/dt/PollySignalR.svg)](https://www.nuget.org/packages/PollySignalR) | Polly v8 reconnect policy for SignalR |
| [PollyElasticsearch](https://www.nuget.org/packages/PollyElasticsearch) | [![Downloads](https://img.shields.io/nuget/dt/PollyElasticsearch.svg)](https://www.nuget.org/packages/PollyElasticsearch) | Polly v8 resilience pipelines for Elastic.Clients.Elasticsearch 8+ — retry, timeout, and circuit-breaker for any Elasticsearch operation, plus a built-in ElasticTransientErrors predicate covering rate limiting (429), service unavailability (503), gateway timeouts (504), and connection failures |
| [PollyHangfire](https://www.nuget.org/packages/PollyHangfire) | [![Downloads](https://img.shields.io/nuget/dt/PollyHangfire.svg)](https://www.nuget.org/packages/PollyHangfire) | Polly v8 resilience pipelines for Hangfire — retry, timeout, and circuit-breaker for IBackgroundJobClient.Enqueue and Schedule |
| [PollyCosmosDb](https://www.nuget.org/packages/PollyCosmosDb) | [![Downloads](https://img.shields.io/nuget/dt/PollyCosmosDb.svg)](https://www.nuget.org/packages/PollyCosmosDb) | Polly v8 resilience pipelines for Azure Cosmos DB — retry, timeout, and circuit-breaker for Container operations, plus a built-in CosmosTransientErrors predicate covering rate limiting (429), timeouts (408), partition failovers (410), and service unavailability (503) |
| [PollySendGrid](https://www.nuget.org/packages/PollySendGrid) | [![Downloads](https://img.shields.io/nuget/dt/PollySendGrid.svg)](https://www.nuget.org/packages/PollySendGrid) | Polly v8 resilience pipelines for SendGrid — retry, timeout, and circuit-breaker for ISendGridClient.SendEmailAsync |
| [PollyMongo](https://www.nuget.org/packages/PollyMongo) | [![Downloads](https://img.shields.io/nuget/dt/PollyMongo.svg)](https://www.nuget.org/packages/PollyMongo) | Polly v8 resilience pipelines for MongoDB.Driver — wrap Find, InsertOne, UpdateOne, DeleteOne and other IMongoCollection calls with retry, timeout, circuit-breaker, and more using a single ResilientMongoCollection decorator |
| [PollyDapper](https://www.nuget.org/packages/PollyDapper) | [![Downloads](https://img.shields.io/nuget/dt/PollyDapper.svg)](https://www.nuget.org/packages/PollyDapper) | Polly v8 resilience pipelines for Dapper — wrap QueryAsync, ExecuteAsync, and other Dapper calls with retry, timeout, circuit-breaker, and more using a single ResilientDbConnection decorator |
| [PollyMediatR](https://www.nuget.org/packages/PollyMediatR) | [![Downloads](https://img.shields.io/nuget/dt/PollyMediatR.svg)](https://www.nuget.org/packages/PollyMediatR) | Polly v8 resilience pipelines for MediatR — add retry, timeout, circuit-breaker, rate-limiting, hedging, and chaos engineering to any MediatR request handler with a single line of DI registration |
| [PollySqlClient](https://www.nuget.org/packages/PollySqlClient) | [![Downloads](https://img.shields.io/nuget/dt/PollySqlClient.svg)](https://www.nuget.org/packages/PollySqlClient) | Polly v8 resilience pipelines for Microsoft.Data.SqlClient (SQL Server and Azure SQL) — retry, timeout, and circuit-breaker for SqlConnection queries and commands, plus a built-in SqlServerTransientErrors predicate covering all common SQL Server and Azure SQL transient error numbers |
| [PollyAzureKeyVault](https://www.nuget.org/packages/PollyAzureKeyVault) | [![Downloads](https://img.shields.io/nuget/dt/PollyAzureKeyVault.svg)](https://www.nuget.org/packages/PollyAzureKeyVault) | Polly v8 resilience pipelines for Azure Key Vault — retry, timeout, and circuit-breaker for SecretClient, KeyClient, and CertificateClient |
| [PollyRedis](https://www.nuget.org/packages/PollyRedis) | [![Downloads](https://img.shields.io/nuget/dt/PollyRedis.svg)](https://www.nuget.org/packages/PollyRedis) | Polly v8 resilience for StackExchange.Redis |
| [PollyAzureServiceBus](https://www.nuget.org/packages/PollyAzureServiceBus) | [![Downloads](https://img.shields.io/nuget/dt/PollyAzureServiceBus.svg)](https://www.nuget.org/packages/PollyAzureServiceBus) | Polly v8 resilience for Azure Service Bus — retry, circuit breaker, and timeout for sending and receiving messages |
| [PollyAzureBlob](https://www.nuget.org/packages/PollyAzureBlob) | [![Downloads](https://img.shields.io/nuget/dt/PollyAzureBlob.svg)](https://www.nuget.org/packages/PollyAzureBlob) | Polly v8 resilience pipelines for Azure Blob Storage — wrap BlobClient and BlobContainerClient operations with retry, timeout, circuit-breaker, and more using ResilientBlobClient and ResilientBlobContainerClient decorators |
| [PollyKafka](https://www.nuget.org/packages/PollyKafka) | [![Downloads](https://img.shields.io/nuget/dt/PollyKafka.svg)](https://www.nuget.org/packages/PollyKafka) | Polly v8 resilience for Confluent.Kafka — retry, circuit breaker, and timeout for producers and consumers |
| [PollyAzureTableStorage](https://www.nuget.org/packages/PollyAzureTableStorage) | [![Downloads](https://img.shields.io/nuget/dt/PollyAzureTableStorage.svg)](https://www.nuget.org/packages/PollyAzureTableStorage) | Polly v8 resilience pipelines for Azure Table Storage — retry, timeout, and circuit-breaker for Azure.Data.Tables TableClient |
| [PollyRateLimiter](https://www.nuget.org/packages/PollyRateLimiter) | [![Downloads](https://img.shields.io/nuget/dt/PollyRateLimiter.svg)](https://www.nuget.org/packages/PollyRateLimiter) | Convenience extension methods for Polly v8 resilience pipelines: AddFixedWindowRateLimiter, AddSlidingWindowRateLimiter, and AddTokenBucketRateLimiter |
| [PollyChaos](https://www.nuget.org/packages/PollyChaos) | [![Downloads](https://img.shields.io/nuget/dt/PollyChaos.svg)](https://www.nuget.org/packages/PollyChaos) | Chaos engineering and fault-injection resilience strategies for Polly v8 pipelines |

## 💼 Need .NET consulting?

The author of this package is available for consulting on **Polly v8 resilience**, **Azure cloud architecture**, and **clean .NET design**.

**[→ solidqualitysolutions.com](https://www.solidqualitysolutions.com/)** · **[LinkedIn](https://www.linkedin.com/in/justbannister/)**
## License

MIT © Justin Bannister
