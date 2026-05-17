public sealed class OrderStateManager(TimeProvider timeProvider)
{
    private static readonly IReadOnlyDictionary<OrderStatus, OrderStatus[]> AllowedTransitions =
        new Dictionary<OrderStatus, OrderStatus[]>
        {
            [OrderStatus.Draft] = [OrderStatus.Submitted, OrderStatus.Cancelled],
            [OrderStatus.Submitted] = [OrderStatus.PaymentAuthorized, OrderStatus.Cancelled],
            [OrderStatus.PaymentAuthorized] = [OrderStatus.InventoryReserved, OrderStatus.Cancelled],
            [OrderStatus.InventoryReserved] = [OrderStatus.FulfillmentQueued, OrderStatus.Cancelled],
            [OrderStatus.FulfillmentQueued] = [OrderStatus.Shipped, OrderStatus.Cancelled],
            [OrderStatus.Shipped] = [],
            [OrderStatus.Cancelled] = [],
        };

    public void Transition(Order order, OrderStatus requestedStatus, string? reason)
    {
        if (
            !AllowedTransitions.TryGetValue(order.Status, out var allowed)
            || !allowed.Contains(requestedStatus)
        )
        {
            throw new InvalidOrderStateTransitionException(
                order.Id,
                order.Status,
                requestedStatus
            );
        }

        order.MoveTo(requestedStatus, timeProvider.GetUtcNow(), reason);
    }
}
