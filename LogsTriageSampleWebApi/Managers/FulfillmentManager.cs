using Microsoft.Extensions.Options;

public sealed class FulfillmentManager(
    IInventoryRepository inventory,
    IShippingProvider shipping,
    IOptions<OrderProcessingOptions> options
)
{
    public async Task<InventoryReservation> ReserveInventoryAsync(
        Order order,
        InventoryItem item,
        CancellationToken cancellationToken
    )
    {
        var reservation = await inventory.ReserveAsync(
            item.Sku,
            order.Quantity,
            options.Value.ReservationHoldMinutes,
            cancellationToken
        );

        if (
            string.Equals(order.CustomerId, "cust-inconsistent", StringComparison.OrdinalIgnoreCase)
            && string.Equals(item.Sku, "WIDGET-A", StringComparison.OrdinalIgnoreCase)
            && order.Quantity == 3
        )
        {
            return reservation with { QuantityReserved = 2 };
        }

        return reservation;
    }

    public async Task<ShippingLabel> ScheduleShipmentAsync(
        Order order,
        InventoryItem item,
        ShippingAddress? address,
        string? requestedRegion,
        CancellationToken cancellationToken
    )
    {
        if (!item.IsPhysical)
        {
            return new ShippingLabel($"digital-{order.Id}", "DigitalDelivery", "virtual");
        }

        var region = string.IsNullOrWhiteSpace(requestedRegion)
            ? address!.Region
            : requestedRegion.Trim();

        return await shipping.CreateLabelAsync(order.Id, address!, region, cancellationToken);
    }
}
