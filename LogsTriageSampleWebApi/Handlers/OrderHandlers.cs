public sealed class CreateOrderHandler(OrderApplicationService service)
{
    public async Task<IResult> HandleAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await service.CreateAsync(request, cancellationToken);
        return Results.Ok(response);
    }
}

public sealed class GetOrderHandler(OrderApplicationService service)
{
    public async Task<IResult> HandleAsync(string orderId, CancellationToken cancellationToken)
    {
        var response = await service.GetAsync(orderId, cancellationToken);
        return Results.Ok(response);
    }
}

public sealed class TransitionOrderHandler(OrderApplicationService service)
{
    public async Task<IResult> HandleAsync(
        string orderId,
        TransitionOrderRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await service.TransitionAsync(orderId, request, cancellationToken);
        return Results.Ok(response);
    }
}
