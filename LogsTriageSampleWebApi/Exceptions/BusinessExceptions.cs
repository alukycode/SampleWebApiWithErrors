public class InputValidationException : Exception
{
    public InputValidationException(IEnumerable<string> errors)
        : base($"Validation failed: {string.Join("; ", errors)}")
    {
        Errors = errors.ToArray();
    }

    public IReadOnlyCollection<string> Errors { get; }
}

public class MissingBusinessDataException : Exception
{
    public MissingBusinessDataException(string message)
        : base(message)
    {
    }
}

public class InvalidBusinessStateException : Exception
{
    public InvalidBusinessStateException(string message)
        : base(message)
    {
    }
}

public class InvalidOrderStateTransitionException : Exception
{
    public InvalidOrderStateTransitionException(
        string orderId,
        OrderStatus currentStatus,
        OrderStatus requestedStatus
    )
        : base(
            $"Order '{orderId}' cannot transition from {currentStatus} to {requestedStatus}."
        )
    {
    }
}

public class RepositoryAccessException : Exception
{
    public RepositoryAccessException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class ExternalProviderException : Exception
{
    public ExternalProviderException(string message)
        : base(message)
    {
    }

    public ExternalProviderException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class TransientProviderException : ExternalProviderException
{
    public TransientProviderException(string message)
        : base(message)
    {
    }
}

public class RetryExhaustedException : Exception
{
    public RetryExhaustedException(string operation, int attempts, Exception innerException)
        : base($"Operation '{operation}' failed after {attempts} retry attempts.", innerException)
    {
        Operation = operation;
        Attempts = attempts;
    }

    public string Operation { get; }

    public int Attempts { get; }
}

public class StrategyResolutionException : Exception
{
    public StrategyResolutionException(string paymentMethod)
        : base($"No payment strategy is registered for payment method '{paymentMethod}'.")
    {
    }
}

public class ConfigurationMissingException : Exception
{
    public ConfigurationMissingException(string key)
        : base($"Required configuration key '{key}' is missing or invalid.")
    {
    }
}

public class InconsistentBusinessStateException : Exception
{
    public InconsistentBusinessStateException(string message)
        : base(message)
    {
    }
}

public class OrderProcessingException : Exception
{
    public OrderProcessingException(string orderDescription, Exception innerException)
        : base($"Order processing failed while handling {orderDescription}.", innerException)
    {
    }
}
