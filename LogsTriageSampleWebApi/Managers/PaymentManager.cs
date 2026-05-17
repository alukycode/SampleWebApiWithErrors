public sealed class PaymentManager(
    IPaymentStrategyResolver strategies,
    RetryPolicy retryPolicy
)
{
    public Task<PaymentAuthorization> AuthorizeAsync(
        Order order,
        BillingProfile billingProfile,
        PriceQuote quote,
        string paymentMethod,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(billingProfile.ProviderCustomerId))
        {
            throw new MissingBusinessDataException(
                $"Order '{order.Id}' has no provider customer id for payment authorization."
            );
        }

        var strategy = strategies.Resolve(paymentMethod);
        return retryPolicy.ExecuteAsync(
            token => strategy.AuthorizeAsync(order, billingProfile, quote, paymentMethod, token),
            $"authorize payment for order {order.Id}",
            cancellationToken
        );
    }
}
