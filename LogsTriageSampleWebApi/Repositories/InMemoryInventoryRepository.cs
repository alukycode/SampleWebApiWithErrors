public sealed class InMemoryInventoryRepository(TimeProvider timeProvider) : IInventoryRepository
{
    private static readonly IReadOnlyDictionary<string, InventoryItem> Inventory =
        new Dictionary<string, InventoryItem>(StringComparer.OrdinalIgnoreCase)
        {
            ["WIDGET-A"] = new("WIDGET-A", "Standard Widget", true, 100, "eastus-primary"),
            ["SUBSCRIPTION-PRO"] = new("SUBSCRIPTION-PRO", "Pro Subscription", false, 999, "digital"),
            ["LOW-STOCK"] = new("LOW-STOCK", "Limited Widget", true, 1, "eastus-primary"),
            ["RESERVATION-FAIL"] = new("RESERVATION-FAIL", "Reservation Edge Case", true, 10, "eastus-primary"),
            ["NEGATIVE-PRICE"] = new("NEGATIVE-PRICE", "Legacy Credit SKU", true, 10, "eastus-primary"),
        };

    public Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        if (string.Equals(sku, "DB-DEADLOCK", StringComparison.OrdinalIgnoreCase))
        {
            throw new RepositoryAccessException(
                "Inventory repository failed while reading SKU availability.",
                new InvalidOperationException("Simulated SQL deadlock victim in Inventory.AvailableStock.")
            );
        }

        Inventory.TryGetValue(sku, out var item);
        return Task.FromResult(item);
    }

    public Task<InventoryReservation> ReserveAsync(
        string sku,
        int quantity,
        int holdMinutes,
        CancellationToken cancellationToken
    )
    {
        if (string.Equals(sku, "RESERVATION-FAIL", StringComparison.OrdinalIgnoreCase))
        {
            throw new RepositoryAccessException(
                "Inventory reservation write failed after availability had already been checked.",
                new InvalidOperationException("Simulated unique key violation on InventoryReservations.")
            );
        }

        return Task.FromResult(
            new InventoryReservation(
                $"res-{Guid.NewGuid():N}"[..16],
                sku,
                quantity,
                timeProvider.GetUtcNow().AddMinutes(holdMinutes)
            )
        );
    }
}
