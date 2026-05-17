public sealed record HealthCheckResponse(
    string Status,
    string Message,
    DateTimeOffset CheckedAt
);

public sealed record DataSubmissionRequest(
    string? Name,
    string? Source,
    string? CorrelationId
);

public sealed record DataSubmissionResponse(
    Guid Id,
    string Name,
    string Source,
    string CorrelationId,
    DateTimeOffset AcceptedAt
);

public sealed record CreateOrderRequest(
    string? CustomerId,
    string? Sku,
    int Quantity,
    string? PaymentMethod,
    string? ShipToRegion,
    string? CouponCode
);

public sealed record OrderResponse(
    string OrderId,
    string CustomerId,
    string Sku,
    int Quantity,
    OrderStatus Status,
    decimal Total,
    string? PaymentAuthorizationId,
    string? ReservationId,
    string? ShippingLabelId
);

public sealed record TransitionOrderRequest(
    OrderStatus TargetStatus,
    string? Reason
);

public sealed record ScenarioInfo(
    string Id,
    string Method,
    string Path,
    string Failure,
    string Example
);
