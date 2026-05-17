using Microsoft.Extensions.Options;

public sealed class RetryPolicy(IOptions<OrderProcessingOptions> options)
{
    public async Task<T> ExecuteAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        string operationName,
        CancellationToken cancellationToken
    )
    {
        var attempts = Math.Max(1, options.Value.Retry.MaxAttempts);
        var delay = Math.Max(0, options.Value.Retry.DelayMilliseconds);
        TransientProviderException? lastFailure = null;

        for (var attempt = 1; attempt <= attempts; attempt++)
        {
            try
            {
                return await operation(cancellationToken);
            }
            catch (TransientProviderException ex) when (attempt < attempts)
            {
                lastFailure = ex;
                if (delay > 0)
                {
                    await Task.Delay(delay, cancellationToken);
                }
            }
            catch (TransientProviderException ex)
            {
                lastFailure = ex;
            }
        }

        throw new RetryExhaustedException(operationName, attempts, lastFailure!);
    }
}
