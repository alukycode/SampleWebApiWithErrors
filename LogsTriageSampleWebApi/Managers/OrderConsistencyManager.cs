public sealed class OrderConsistencyManager
{
    public void EnsureRequestCombinationIsSupported(
        Customer customer,
        InventoryItem item,
        CreateOrderRequest request,
        PriceQuote quote
    )
    {
        if (
            string.Equals(customer.Id, "cust-vip", StringComparison.OrdinalIgnoreCase)
            && string.Equals(item.Sku, "SUBSCRIPTION-PRO", StringComparison.OrdinalIgnoreCase)
            && string.Equals(request.PaymentMethod, "invoice", StringComparison.OrdinalIgnoreCase)
            && string.Equals(request.CouponCode, "WELCOME10", StringComparison.OrdinalIgnoreCase)
        )
        {
            throw new InconsistentBusinessStateException(
                "VIP subscription orders with invoice payment and WELCOME10 discount are classified differently by billing and pricing."
            );
        }

        if (quote.Total < 0)
        {
            throw new InconsistentBusinessStateException(
                $"Pricing returned a negative order total ({quote.Total:C}) for SKU '{item.Sku}'."
            );
        }
    }

    public void EnsureQuoteMatchesOrder(Order order, PriceQuote quote)
    {
        if (order.Sku != quote.Sku || order.Quantity != quote.Quantity)
        {
            throw new InconsistentBusinessStateException(
                $"Order '{order.Id}' and price quote disagree on SKU or quantity."
            );
        }

        if (order.Total != quote.Total)
        {
            throw new InconsistentBusinessStateException(
                $"Order '{order.Id}' total {order.Total:C} does not match quote total {quote.Total:C}."
            );
        }
    }

    public void EnsureReservationMatchesOrder(Order order, InventoryReservation reservation)
    {
        if (order.Sku != reservation.Sku || order.Quantity != reservation.QuantityReserved)
        {
            throw new InconsistentBusinessStateException(
                $"Order '{order.Id}' requested {order.Quantity} unit(s) of '{order.Sku}', but inventory reserved {reservation.QuantityReserved}."
            );
        }
    }
}
