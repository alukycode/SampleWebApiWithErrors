using Microsoft.AspNetCore.Mvc;

public static class OrderEndpointExtensions
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var orders = endpoints.MapGroup("/api/orders")
            .WithTags("Orders");

        orders.MapGet(
                "/scenarios",
                () => Results.Ok(OrderScenarioCatalog.All)
            )
            .WithSummary("Lists order exception scenarios")
            .WithDescription("Returns business-flow inputs that intentionally reproduce layered runtime failures.");

        orders.MapPost(
                "/",
                (
                    [FromServices] CreateOrderHandler handler,
                    CreateOrderRequest request,
                    CancellationToken cancellationToken
                ) => handler.HandleAsync(request, cancellationToken)
            )
            .WithSummary("Creates an order")
            .WithDescription("Runs validation, customer lookup, pricing, payment, inventory, fulfillment, and persistence.");

        orders.MapGet(
                "/{orderId}",
                (
                    [FromServices] GetOrderHandler handler,
                    string orderId,
                    CancellationToken cancellationToken
                ) => handler.HandleAsync(orderId, cancellationToken)
            )
            .WithSummary("Gets an order")
            .WithDescription("Loads an order through the repository-backed application service.");

        orders.MapPost(
                "/{orderId}/transitions",
                (
                    [FromServices] TransitionOrderHandler handler,
                    string orderId,
                    TransitionOrderRequest request,
                    CancellationToken cancellationToken
                ) => handler.HandleAsync(orderId, request, cancellationToken)
            )
            .WithSummary("Transitions an order")
            .WithDescription("Applies an order state transition through the state manager.");

        return endpoints;
    }
}
