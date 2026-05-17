using System.Collections.Concurrent;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<string, Order> orders =
        new(StringComparer.OrdinalIgnoreCase);

    public InMemoryOrderRepository(TimeProvider timeProvider)
    {
        var draft = new Order("ord-draft", "cust-ok", "WIDGET-A", 1, 26.99m);
        orders[draft.Id] = draft;

        var shipped = new Order("ord-shipped", "cust-ok", "WIDGET-A", 1, 26.99m);
        shipped.MoveTo(OrderStatus.Submitted, timeProvider.GetUtcNow().AddHours(-3), "Seeded order.");
        shipped.MoveTo(OrderStatus.PaymentAuthorized, timeProvider.GetUtcNow().AddHours(-2), "Seeded order.");
        shipped.MoveTo(OrderStatus.InventoryReserved, timeProvider.GetUtcNow().AddHours(-1), "Seeded order.");
        shipped.MoveTo(OrderStatus.FulfillmentQueued, timeProvider.GetUtcNow().AddMinutes(-30), "Seeded order.");
        shipped.MoveTo(OrderStatus.Shipped, timeProvider.GetUtcNow().AddMinutes(-5), "Seeded order.");
        orders[shipped.Id] = shipped;
    }

    public Task<Order?> GetByIdAsync(string orderId, CancellationToken cancellationToken)
    {
        orders.TryGetValue(orderId, out var order);
        return Task.FromResult(order);
    }

    public Task SaveAsync(Order order, CancellationToken cancellationToken)
    {
        if (string.Equals(order.CustomerId, "cust-save-fails", StringComparison.OrdinalIgnoreCase))
        {
            throw new RepositoryAccessException(
                $"Order repository failed while saving order '{order.Id}'.",
                new IOException("Simulated disk full error while appending to the order event stream.")
            );
        }

        orders[order.Id] = order;
        return Task.CompletedTask;
    }
}
