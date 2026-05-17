public sealed class OrderProcessingOptions
{
    public const string SectionName = "OrderProcessing";

    public string DefaultWarehouse { get; set; } = "eastus-primary";

    public string PaymentGatewayName { get; set; } = "ContosoPay";

    public int ReservationHoldMinutes { get; set; } = 20;

    public RetryOptions Retry { get; set; } = new();

    public FraudOptions Fraud { get; set; } = new();
}

public sealed class RetryOptions
{
    public int MaxAttempts { get; set; } = 3;

    public int DelayMilliseconds { get; set; } = 15;
}

public sealed class FraudOptions
{
    public decimal? RiskThreshold { get; set; }
}
