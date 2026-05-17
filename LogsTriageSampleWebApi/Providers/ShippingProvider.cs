public interface IShippingProvider
{
    Task<ShippingLabel> CreateLabelAsync(
        string orderId,
        ShippingAddress address,
        string region,
        CancellationToken cancellationToken
    );
}

public sealed class ShippingProvider : IShippingProvider
{
    public async Task<ShippingLabel> CreateLabelAsync(
        string orderId,
        ShippingAddress address,
        string region,
        CancellationToken cancellationToken
    )
    {
        if (string.Equals(region, "timeout", StringComparison.OrdinalIgnoreCase))
        {
            await Task.Delay(75, cancellationToken);
            throw new TimeoutException(
                $"Carrier service timed out while creating a label for order '{orderId}'."
            );
        }

        if (string.Equals(region, "provider-down", StringComparison.OrdinalIgnoreCase))
        {
            throw new ExternalProviderException(
                $"Carrier service rejected shipment to '{address.Country}/{region}'.",
                new HttpRequestException("Carrier API returned HTTP 503 Service Unavailable.")
            );
        }

        if (string.Equals(region, "unsupported-region", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidBusinessStateException(
                $"No carrier contract is available for region '{region}'."
            );
        }

        return new ShippingLabel($"ship-{Guid.NewGuid():N}"[..17], "FabrikamShip", region);
    }
}
