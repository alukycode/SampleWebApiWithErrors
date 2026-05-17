public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(string customerId, CancellationToken cancellationToken);
}

public interface IInventoryRepository
{
    Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken);

    Task<InventoryReservation> ReserveAsync(
        string sku,
        int quantity,
        int holdMinutes,
        CancellationToken cancellationToken
    );
}

public interface IPricingRepository
{
    Task<decimal?> GetUnitPriceAsync(string sku, CancellationToken cancellationToken);
}

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(string orderId, CancellationToken cancellationToken);

    Task SaveAsync(Order order, CancellationToken cancellationToken);
}

public interface IDataSubmissionRepository
{
    Task SaveAsync(DataSubmissionResponse submission, CancellationToken cancellationToken);
}
