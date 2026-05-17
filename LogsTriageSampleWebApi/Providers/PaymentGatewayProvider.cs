using Microsoft.Extensions.Options;

public interface IPaymentProvider
{
    Task<PaymentAuthorization> AuthorizeAsync(
        Order order,
        BillingProfile billingProfile,
        PriceQuote quote,
        string paymentMethod,
        CancellationToken cancellationToken
    );
}

public sealed class PaymentGatewayProvider(IOptions<OrderProcessingOptions> options) : IPaymentProvider
{
    public async Task<PaymentAuthorization> AuthorizeAsync(
        Order order,
        BillingProfile billingProfile,
        PriceQuote quote,
        string paymentMethod,
        CancellationToken cancellationToken
    )
    {
        var gatewayName = options.Value.PaymentGatewayName;

        if (string.Equals(paymentMethod, "flaky-card", StringComparison.OrdinalIgnoreCase))
        {
            throw new TransientProviderException(
                $"{gatewayName} returned HTTP 503 while authorizing order '{order.Id}'."
            );
        }

        if (string.Equals(paymentMethod, "declined-network", StringComparison.OrdinalIgnoreCase))
        {
            throw new ExternalProviderException(
                $"{gatewayName} returned a malformed decline response for customer '{billingProfile.ProviderCustomerId}'."
            );
        }

        if (string.Equals(paymentMethod, "wrapped-card", StringComparison.OrdinalIgnoreCase))
        {
            throw new ExternalProviderException(
                $"{gatewayName} failed while normalizing the authorization response.",
                new HttpRequestException("Remote gateway returned HTTP 502 with an empty response body.")
            );
        }

        if (string.Equals(paymentMethod, "timeout-card", StringComparison.OrdinalIgnoreCase))
        {
            await Task.Delay(50, cancellationToken);
            throw new TimeoutException(
                $"{gatewayName} did not respond before the simulated authorization timeout."
            );
        }

        return new PaymentAuthorization(
            $"auth-{Guid.NewGuid():N}"[..17],
            gatewayName,
            quote.Total,
            paymentMethod
        );
    }
}
