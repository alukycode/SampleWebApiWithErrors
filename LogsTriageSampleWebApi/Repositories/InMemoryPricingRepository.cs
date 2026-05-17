public sealed class InMemoryPricingRepository : IPricingRepository
{
    private static readonly IReadOnlyDictionary<string, decimal> Prices =
        new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            ["WIDGET-A"] = 24.99m,
            ["SUBSCRIPTION-PRO"] = 49.99m,
            ["LOW-STOCK"] = 199.99m,
            ["RESERVATION-FAIL"] = 14.99m,
            ["NEGATIVE-PRICE"] = -5.00m,
        };

    public Task<decimal?> GetUnitPriceAsync(string sku, CancellationToken cancellationToken)
    {
        if (string.Equals(sku, "PRICE-TABLE-OFFLINE", StringComparison.OrdinalIgnoreCase))
        {
            throw new RepositoryAccessException(
                "Pricing repository could not query the active price table.",
                new InvalidOperationException("Simulated read replica unavailable for Pricing.ActivePrices.")
            );
        }

        if (Prices.TryGetValue(sku, out var price))
        {
            return Task.FromResult<decimal?>(price);
        }

        return Task.FromResult<decimal?>(null);
    }
}
