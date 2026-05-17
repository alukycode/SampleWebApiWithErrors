public sealed class InventoryManager(IInventoryRepository inventory)
{
    public async Task<InventoryItem> GetAvailableItemAsync(
        string sku,
        int quantity,
        CancellationToken cancellationToken
    )
    {
        var item = await inventory.GetBySkuAsync(sku, cancellationToken)
            ?? throw new MissingBusinessDataException($"SKU '{sku}' was not found.");

        if (item.AvailableQuantity < quantity)
        {
            throw new InvalidBusinessStateException(
                $"SKU '{sku}' has {item.AvailableQuantity} units available, but {quantity} were requested."
            );
        }

        return item;
    }
}
