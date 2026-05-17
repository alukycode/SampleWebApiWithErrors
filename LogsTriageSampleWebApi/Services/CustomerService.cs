public sealed class CustomerService(ICustomerRepository customers)
{
    public async Task<Customer> GetActiveCustomerAsync(
        string customerId,
        CancellationToken cancellationToken
    )
    {
        var customer = await customers.GetByIdAsync(customerId, cancellationToken)
            ?? throw new MissingBusinessDataException($"Customer '{customerId}' was not found.");

        return customer.Status switch
        {
            CustomerStatus.Active => customer,
            CustomerStatus.Suspended => throw new InvalidBusinessStateException(
                $"Customer '{customer.Id}' is suspended and cannot place new orders."
            ),
            CustomerStatus.Closed => throw new InvalidBusinessStateException(
                $"Customer '{customer.Id}' is closed and cannot place new orders."
            ),
            _ => throw new InvalidBusinessStateException(
                $"Customer '{customer.Id}' has unknown status '{customer.Status}'."
            ),
        };
    }
}
