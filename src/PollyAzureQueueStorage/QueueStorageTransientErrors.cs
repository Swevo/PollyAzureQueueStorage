/// <summary>
/// Pre-built Polly <see cref="PredicateBuilder"/> for transient Azure Queue Storage errors.
/// Covers throttling (429), service unavailability (503), and gateway timeouts (504).
/// </summary>
public static class QueueStorageTransientErrors
{
    /// <summary>HTTP status codes returned by Azure Queue Storage that are safe to retry.</summary>
    public static readonly IReadOnlySet<int> StatusCodes = new HashSet<int>
    {
        429, // TooManyRequests — storage account throttle limit reached
        503, // ServiceUnavailable — service maintenance or regional outage
        504, // GatewayTimeout — proxy or load balancer timed out
    };

    /// <summary>
    /// A <see cref="PredicateBuilder"/> that handles <see cref="RequestFailedException"/>
    /// with a status code in <see cref="StatusCodes"/>, <see cref="HttpRequestException"/>,
    /// and <see cref="TaskCanceledException"/>.
    /// </summary>
    public static readonly PredicateBuilder IsTransient =
        (PredicateBuilder)new PredicateBuilder()
            .Handle<RequestFailedException>(ex => StatusCodes.Contains(ex.Status))
            .Handle<HttpRequestException>()
            .Handle<TaskCanceledException>();
}
