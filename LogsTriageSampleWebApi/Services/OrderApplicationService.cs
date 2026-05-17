public sealed class OrderApplicationService(
    CustomerService customers,
    InventoryManager inventory,
    PricingManager pricing,
    PaymentManager payments,
    FulfillmentManager fulfillment,
    OrderConsistencyManager consistency,
    OrderStateManager states,
    IOrderRepository orders
)
{
    public async Task<OrderResponse> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken
    )
    {
        Validate(request);

        var description = $"customer '{request.CustomerId}' and SKU '{request.Sku}'";

        try
        {
            var customer = await customers.GetActiveCustomerAsync(
                request.CustomerId!,
                cancellationToken
            );
            var item = await inventory.GetAvailableItemAsync(
                request.Sku!,
                request.Quantity,
                cancellationToken
            );
            var quote = await pricing.CalculateAsync(
                customer,
                item,
                request.Quantity,
                request.CouponCode,
                cancellationToken
            );

            consistency.EnsureRequestCombinationIsSupported(customer, item, request, quote);

            var order = new Order(
                $"ord-{Guid.NewGuid():N}"[..16],
                customer.Id,
                item.Sku,
                request.Quantity,
                quote.Total
            );

            states.Transition(order, OrderStatus.Submitted, "Order accepted from API.");
            consistency.EnsureQuoteMatchesOrder(order, quote);

            var authorization = await payments.AuthorizeAsync(
                order,
                customer.BillingProfile!,
                quote,
                request.PaymentMethod!,
                cancellationToken
            );
            order.RecordPayment(authorization);
            states.Transition(order, OrderStatus.PaymentAuthorized, "Payment authorized.");

            var reservation = await fulfillment.ReserveInventoryAsync(
                order,
                item,
                cancellationToken
            );
            consistency.EnsureReservationMatchesOrder(order, reservation);
            order.RecordReservation(reservation);
            states.Transition(order, OrderStatus.InventoryReserved, "Inventory reserved.");

            var label = await fulfillment.ScheduleShipmentAsync(
                order,
                item,
                customer.ShippingAddress,
                request.ShipToRegion,
                cancellationToken
            );
            order.RecordShippingLabel(label);
            states.Transition(order, OrderStatus.FulfillmentQueued, "Fulfillment queued.");

            await orders.SaveAsync(order, cancellationToken);
            return ToResponse(order);
        }
        catch (RepositoryAccessException ex)
        {
            throw new OrderProcessingException(description, ex);
        }
        catch (ExternalProviderException ex)
        {
            throw new OrderProcessingException(description, ex);
        }
        catch (RetryExhaustedException ex)
        {
            throw new OrderProcessingException(description, ex);
        }
    }

    public async Task<OrderResponse> GetAsync(string orderId, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            throw new InputValidationException(["Order id is required."]);
        }

        var order = await orders.GetByIdAsync(orderId, cancellationToken)
            ?? throw new MissingBusinessDataException($"Order '{orderId}' was not found.");

        return ToResponse(order);
    }

    public async Task<OrderResponse> TransitionAsync(
        string orderId,
        TransitionOrderRequest request,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(orderId))
        {
            throw new InputValidationException(["Order id is required."]);
        }

        var order = await orders.GetByIdAsync(orderId, cancellationToken)
            ?? throw new MissingBusinessDataException($"Order '{orderId}' was not found.");

        states.Transition(order, request.TargetStatus, request.Reason);
        await orders.SaveAsync(order, cancellationToken);

        return ToResponse(order);
    }

    private static void Validate(CreateOrderRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.CustomerId))
        {
            errors.Add("CustomerId is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Sku))
        {
            errors.Add("Sku is required.");
        }

        if (request.Quantity <= 0)
        {
            errors.Add("Quantity must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(request.PaymentMethod))
        {
            errors.Add("PaymentMethod is required.");
        }

        if (errors.Count > 0)
        {
            throw new InputValidationException(errors);
        }
    }

    private static OrderResponse ToResponse(Order order) =>
        new(
            order.Id,
            order.CustomerId,
            order.Sku,
            order.Quantity,
            order.Status,
            order.Total,
            order.PaymentAuthorizationId,
            order.ReservationId,
            order.ShippingLabelId
        );
}
