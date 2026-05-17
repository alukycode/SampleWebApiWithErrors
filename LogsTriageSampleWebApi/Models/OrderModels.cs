public enum CustomerStatus
{
    Active,
    Suspended,
    Closed,
}

public enum OrderStatus
{
    Draft,
    Submitted,
    PaymentAuthorized,
    InventoryReserved,
    FulfillmentQueued,
    Shipped,
    Cancelled,
}

public sealed record BillingProfile(
    string ProviderCustomerId,
    string DefaultPaymentToken,
    decimal InvoiceLimit
);

public sealed record ShippingAddress(
    string Line1,
    string City,
    string Country,
    string Region
);

public sealed record Customer(
    string Id,
    string Name,
    CustomerStatus Status,
    BillingProfile? BillingProfile,
    ShippingAddress? ShippingAddress,
    bool IsTaxExempt
);

public sealed record InventoryItem(
    string Sku,
    string Name,
    bool IsPhysical,
    int AvailableQuantity,
    string WarehouseCode
);

public sealed record PriceQuote(
    string Sku,
    int Quantity,
    decimal UnitPrice,
    decimal Discount,
    decimal Tax,
    decimal Total
);

public sealed record InventoryReservation(
    string ReservationId,
    string Sku,
    int QuantityReserved,
    DateTimeOffset ExpiresAt
);

public sealed record PaymentAuthorization(
    string AuthorizationId,
    string Provider,
    decimal Amount,
    string Method
);

public sealed record ShippingLabel(
    string LabelId,
    string Carrier,
    string Region
);

public sealed record OrderStatusChange(
    OrderStatus From,
    OrderStatus To,
    DateTimeOffset ChangedAt,
    string? Reason
);

public sealed class Order
{
    public Order(
        string id,
        string customerId,
        string sku,
        int quantity,
        decimal total
    )
    {
        Id = id;
        CustomerId = customerId;
        Sku = sku;
        Quantity = quantity;
        Total = total;
    }

    public string Id { get; }

    public string CustomerId { get; }

    public string Sku { get; }

    public int Quantity { get; }

    public decimal Total { get; }

    public OrderStatus Status { get; private set; } = OrderStatus.Draft;

    public string? PaymentAuthorizationId { get; private set; }

    public string? ReservationId { get; private set; }

    public string? ShippingLabelId { get; private set; }

    public List<OrderStatusChange> History { get; } = [];

    public void MoveTo(OrderStatus status, DateTimeOffset changedAt, string? reason)
    {
        History.Add(new OrderStatusChange(Status, status, changedAt, reason));
        Status = status;
    }

    public void RecordPayment(PaymentAuthorization authorization)
    {
        PaymentAuthorizationId = authorization.AuthorizationId;
    }

    public void RecordReservation(InventoryReservation reservation)
    {
        ReservationId = reservation.ReservationId;
    }

    public void RecordShippingLabel(ShippingLabel label)
    {
        ShippingLabelId = label.LabelId;
    }
}
