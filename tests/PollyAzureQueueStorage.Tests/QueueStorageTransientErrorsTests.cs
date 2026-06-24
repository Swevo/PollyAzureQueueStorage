public class QueueStorageTransientErrorsTests
{
    [Theory]
    [InlineData(429)]
    [InlineData(503)]
    [InlineData(504)]
    public void StatusCodes_ContainsTransientCode(int code)
        => Assert.Contains(code, QueueStorageTransientErrors.StatusCodes);

    [Theory]
    [InlineData(200)]
    [InlineData(400)]
    [InlineData(403)]
    [InlineData(404)]
    [InlineData(500)]
    public void StatusCodes_DoesNotContainNonTransientCode(int code)
        => Assert.DoesNotContain(code, QueueStorageTransientErrors.StatusCodes);

    [Fact]
    public void StatusCodes_HasThreeEntries()
        => Assert.Equal(3, QueueStorageTransientErrors.StatusCodes.Count);

    [Fact]
    public void IsTransient_IsNotNull()
        => Assert.NotNull(QueueStorageTransientErrors.IsTransient);

    [Theory]
    [InlineData(429)]
    [InlineData(503)]
    [InlineData(504)]
    public async Task IsTransient_RetriesTransientRequestFailedException(int status)
    {
        var pipeline = BuildRetryPipeline();
        var attempts = 0;
        await Assert.ThrowsAsync<RequestFailedException>(() =>
            pipeline.ExecuteAsync(ct => { attempts++; throw new RequestFailedException(status, "transient"); }).AsTask());
        Assert.Equal(2, attempts);
    }

    [Theory]
    [InlineData(400)]
    [InlineData(403)]
    [InlineData(404)]
    public async Task IsTransient_DoesNotRetryNonTransientRequestFailedException(int status)
    {
        var pipeline = BuildRetryPipeline();
        var attempts = 0;
        await Assert.ThrowsAsync<RequestFailedException>(() =>
            pipeline.ExecuteAsync(ct => { attempts++; throw new RequestFailedException(status, "perm"); }).AsTask());
        Assert.Equal(1, attempts);
    }

    private static ResiliencePipeline BuildRetryPipeline() =>
        new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions { MaxRetryAttempts = 1, Delay = TimeSpan.Zero, ShouldHandle = QueueStorageTransientErrors.IsTransient })
            .Build();
}
