public static class OrderScenarioCatalog
{
    public static IReadOnlyList<ScenarioInfo> All { get; } =
    [
        new(
            "happy-path",
            "POST",
            "/api/orders",
            "No exception; creates a normal order.",
            """{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"credit-card","shipToRegion":"west"}"""
        ),
        new(
            "validation-failure",
            "POST",
            "/api/orders",
            "InputValidationException from the application service validator.",
            """{"customerId":"","sku":"WIDGET-A","quantity":0,"paymentMethod":"credit-card"}"""
        ),
        new(
            "missing-customer",
            "POST",
            "/api/orders",
            "MissingBusinessDataException when the customer repository returns no customer.",
            """{"customerId":"cust-missing","sku":"WIDGET-A","quantity":1,"paymentMethod":"credit-card"}"""
        ),
        new(
            "null-billing-profile",
            "POST",
            "/api/orders",
            "NullReferenceException caused by incomplete customer data reaching the payment manager.",
            """{"customerId":"cust-null-billing","sku":"WIDGET-A","quantity":1,"paymentMethod":"credit-card"}"""
        ),
        new(
            "invalid-customer-state",
            "POST",
            "/api/orders",
            "InvalidBusinessStateException for a suspended customer.",
            """{"customerId":"cust-suspended","sku":"WIDGET-A","quantity":1,"paymentMethod":"credit-card"}"""
        ),
        new(
            "repository-deadlock",
            "POST",
            "/api/orders",
            "OrderProcessingException wrapping RepositoryAccessException and a simulated data-store error.",
            """{"customerId":"cust-ok","sku":"DB-DEADLOCK","quantity":1,"paymentMethod":"credit-card"}"""
        ),
        new(
            "external-payment-failure",
            "POST",
            "/api/orders",
            "OrderProcessingException wrapping ExternalProviderException from the payment provider.",
            """{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"declined-network"}"""
        ),
        new(
            "payment-timeout",
            "POST",
            "/api/orders",
            "TimeoutException from the simulated payment gateway.",
            """{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"timeout-card"}"""
        ),
        new(
            "retry-exhaustion",
            "POST",
            "/api/orders",
            "OrderProcessingException wrapping RetryExhaustedException after repeated transient provider failures.",
            """{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"flaky-card"}"""
        ),
        new(
            "strategy-resolution",
            "POST",
            "/api/orders",
            "StrategyResolutionException when no payment strategy matches the request.",
            """{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"crypto"}"""
        ),
        new(
            "configuration-missing",
            "POST",
            "/api/orders",
            "ConfigurationMissingException when a coupon requires missing fraud settings.",
            """{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"credit-card","couponCode":"RISKY"}"""
        ),
        new(
            "inconsistent-reservation",
            "POST",
            "/api/orders",
            "InconsistentBusinessStateException when inventory reservation disagrees with the order.",
            """{"customerId":"cust-inconsistent","sku":"WIDGET-A","quantity":3,"paymentMethod":"credit-card"}"""
        ),
        new(
            "specific-input-combination",
            "POST",
            "/api/orders",
            "InconsistentBusinessStateException for a specific VIP subscription/invoice/coupon combination.",
            """{"customerId":"cust-vip","sku":"SUBSCRIPTION-PRO","quantity":1,"paymentMethod":"invoice","couponCode":"WELCOME10"}"""
        ),
        new(
            "shipping-provider-chain",
            "POST",
            "/api/orders",
            "OrderProcessingException wrapping ExternalProviderException with HttpRequestException inner cause.",
            """{"customerId":"cust-ok","sku":"WIDGET-A","quantity":1,"paymentMethod":"credit-card","shipToRegion":"provider-down"}"""
        ),
        new(
            "invalid-state-transition",
            "POST",
            "/api/orders/ord-shipped/transitions",
            "InvalidOrderStateTransitionException when a shipped order is cancelled.",
            """{"targetStatus":"Cancelled","reason":"customer requested cancellation"}"""
        ),
    ];
}
