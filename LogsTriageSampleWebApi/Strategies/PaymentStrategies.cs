public interface IPaymentStrategy
{
    bool CanHandle(string paymentMethod);

    Task<PaymentAuthorization> AuthorizeAsync(
        Order order,
        BillingProfile billingProfile,
        PriceQuote quote,
        string paymentMethod,
        CancellationToken cancellationToken
    );
}

public interface IPaymentStrategyResolver
{
    IPaymentStrategy Resolve(string paymentMethod);
}

public sealed class CreditCardPaymentStrategy(IPaymentProvider provider) : IPaymentStrategy
{
    private static readonly string[] SupportedMethods =
    [
        "credit-card",
        "card",
        "flaky-card",
        "timeout-card",
        "declined-network",
        "wrapped-card",
    ];

    public bool CanHandle(string paymentMethod) =>
        SupportedMethods.Contains(paymentMethod, StringComparer.OrdinalIgnoreCase);

    public Task<PaymentAuthorization> AuthorizeAsync(
        Order order,
        BillingProfile billingProfile,
        PriceQuote quote,
        string paymentMethod,
        CancellationToken cancellationToken
    ) =>
        provider.AuthorizeAsync(order, billingProfile, quote, paymentMethod, cancellationToken);
}

public sealed class InvoicePaymentStrategy : IPaymentStrategy
{
    public bool CanHandle(string paymentMethod) =>
        string.Equals(paymentMethod, "invoice", StringComparison.OrdinalIgnoreCase);

    public Task<PaymentAuthorization> AuthorizeAsync(
        Order order,
        BillingProfile billingProfile,
        PriceQuote quote,
        string paymentMethod,
        CancellationToken cancellationToken
    )
    {
        if (quote.Total > billingProfile.InvoiceLimit)
        {
            throw new InvalidBusinessStateException(
                $"Order '{order.Id}' total {quote.Total:C} exceeds invoice limit {billingProfile.InvoiceLimit:C}."
            );
        }

        return Task.FromResult(
            new PaymentAuthorization(
                $"inv-{Guid.NewGuid():N}"[..16],
                "InternalInvoice",
                quote.Total,
                paymentMethod
            )
        );
    }
}

public sealed class WalletPaymentStrategy : IPaymentStrategy
{
    public bool CanHandle(string paymentMethod) =>
        string.Equals(paymentMethod, "wallet", StringComparison.OrdinalIgnoreCase);

    public Task<PaymentAuthorization> AuthorizeAsync(
        Order order,
        BillingProfile billingProfile,
        PriceQuote quote,
        string paymentMethod,
        CancellationToken cancellationToken
    )
    {
        if (string.Equals(billingProfile.DefaultPaymentToken, "wallet-empty", StringComparison.OrdinalIgnoreCase))
        {
            throw new ExternalProviderException(
                $"Wallet provider reported insufficient funds for order '{order.Id}'."
            );
        }

        return Task.FromResult(
            new PaymentAuthorization(
                $"wal-{Guid.NewGuid():N}"[..16],
                "InternalWallet",
                quote.Total,
                paymentMethod
            )
        );
    }
}

public sealed class PaymentStrategyResolver(IEnumerable<IPaymentStrategy> strategies)
    : IPaymentStrategyResolver
{
    public IPaymentStrategy Resolve(string paymentMethod)
    {
        var strategy = strategies.FirstOrDefault(strategy => strategy.CanHandle(paymentMethod));
        return strategy ?? throw new StrategyResolutionException(paymentMethod);
    }
}
