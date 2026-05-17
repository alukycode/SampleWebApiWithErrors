using Microsoft.Extensions.Options;

public sealed class PricingManager(
    IPricingRepository pricing,
    IOptions<OrderProcessingOptions> options
)
{
    public async Task<PriceQuote> CalculateAsync(
        Customer customer,
        InventoryItem item,
        int quantity,
        string? couponCode,
        CancellationToken cancellationToken
    )
    {
        var unitPrice = await pricing.GetUnitPriceAsync(item.Sku, cancellationToken)
            ?? throw new MissingBusinessDataException(
                $"No active price could be found for SKU '{item.Sku}'."
            );

        var subtotal = unitPrice * quantity;
        var discount = CalculateDiscount(customer, couponCode, subtotal);
        var taxableAmount = subtotal - discount;
        var tax = customer.IsTaxExempt ? 0 : Math.Round(taxableAmount * 0.0825m, 2);
        var total = taxableAmount + tax;

        return new PriceQuote(item.Sku, quantity, unitPrice, discount, tax, total);
    }

    private decimal CalculateDiscount(Customer customer, string? couponCode, decimal subtotal)
    {
        if (string.IsNullOrWhiteSpace(couponCode))
        {
            return 0;
        }

        return couponCode.Trim().ToUpperInvariant() switch
        {
            "WELCOME10" => Math.Round(subtotal * 0.10m, 2),
            "VIP25" when string.Equals(customer.Id, "cust-vip", StringComparison.OrdinalIgnoreCase) =>
                Math.Round(subtotal * 0.25m, 2),
            "VIP25" => throw new InvalidBusinessStateException(
                $"Coupon '{couponCode}' is only valid for VIP customers."
            ),
            "EXPIRED" => throw new InvalidBusinessStateException(
                $"Coupon '{couponCode}' expired before the order was submitted."
            ),
            "RISKY" => CalculateRiskAdjustedDiscount(subtotal),
            _ => throw new MissingBusinessDataException(
                $"Coupon '{couponCode}' was not found in the pricing catalog."
            ),
        };
    }

    private decimal CalculateRiskAdjustedDiscount(decimal subtotal)
    {
        var threshold = options.Value.Fraud.RiskThreshold
            ?? throw new ConfigurationMissingException(
                $"{OrderProcessingOptions.SectionName}:Fraud:RiskThreshold"
            );

        return Math.Round(subtotal * Math.Min(threshold, 0.35m), 2);
    }
}
