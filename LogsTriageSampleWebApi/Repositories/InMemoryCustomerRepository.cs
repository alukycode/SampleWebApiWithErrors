public sealed class InMemoryCustomerRepository : ICustomerRepository
{
    private static readonly IReadOnlyDictionary<string, Customer> Customers =
        new Dictionary<string, Customer>(StringComparer.OrdinalIgnoreCase)
        {
            ["cust-ok"] = new(
                "cust-ok",
                "Northwind Operations",
                CustomerStatus.Active,
                new BillingProfile("pay_cust_ok", "tok_visa_ok", 2_500),
                new ShippingAddress("1 Market St", "Seattle", "US", "west"),
                false
            ),
            ["cust-vip"] = new(
                "cust-vip",
                "Contoso Strategic",
                CustomerStatus.Active,
                new BillingProfile("pay_cust_vip", "tok_invoice_ok", 10_000),
                new ShippingAddress("100 Enterprise Way", "New York", "US", "east"),
                false
            ),
            ["cust-null-billing"] = new(
                "cust-null-billing",
                "Legacy Billing Import",
                CustomerStatus.Active,
                null,
                new ShippingAddress("44 Null Island Rd", "Austin", "US", "south"),
                false
            ),
            ["cust-null-address"] = new(
                "cust-null-address",
                "Warehouse Pickup Only",
                CustomerStatus.Active,
                new BillingProfile("pay_no_address", "tok_visa_ok", 500),
                null,
                false
            ),
            ["cust-suspended"] = new(
                "cust-suspended",
                "Past Due Customer",
                CustomerStatus.Suspended,
                new BillingProfile("pay_suspended", "tok_visa_ok", 100),
                new ShippingAddress("9 Hold Ln", "Chicago", "US", "midwest"),
                false
            ),
            ["cust-inconsistent"] = new(
                "cust-inconsistent",
                "Split Brain Logistics",
                CustomerStatus.Active,
                new BillingProfile("pay_split_brain", "tok_visa_ok", 1_000),
                new ShippingAddress("7 Race Condition Ave", "Denver", "US", "west"),
                false
            ),
            ["cust-save-fails"] = new(
                "cust-save-fails",
                "Persistence Failure Account",
                CustomerStatus.Active,
                new BillingProfile("pay_save_fails", "tok_visa_ok", 1_000),
                new ShippingAddress("12 Disk Full Dr", "Boston", "US", "east"),
                false
            ),
        };

    public Task<Customer?> GetByIdAsync(string customerId, CancellationToken cancellationToken)
    {
        if (string.Equals(customerId, "cust-repo-timeout", StringComparison.OrdinalIgnoreCase))
        {
            throw new RepositoryAccessException(
                "Customer repository timed out while reading the customer profile.",
                new TimeoutException("Simulated customer database timeout after 30 seconds.")
            );
        }

        if (string.Equals(customerId, "cust-json-corrupt", StringComparison.OrdinalIgnoreCase))
        {
            throw new RepositoryAccessException(
                "Customer repository returned an unreadable billing profile document.",
                new FormatException("The stored billingProfile JSON payload is malformed.")
            );
        }

        Customers.TryGetValue(customerId, out var customer);
        return Task.FromResult(customer);
    }
}
