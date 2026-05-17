public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddSampleApplication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<OrderProcessingOptions>(
            configuration.GetSection(OrderProcessingOptions.SectionName)
        );

        services.AddSingleton(TimeProvider.System);

        services.AddScoped<HealthCheckHandler>();
        services.AddScoped<SubmitDataHandler>();
        services.AddScoped<DiagnosticsProbeHandler>();
        services.AddScoped<CreateOrderHandler>();
        services.AddScoped<GetOrderHandler>();
        services.AddScoped<TransitionOrderHandler>();

        services.AddScoped<HealthCheckService>();
        services.AddScoped<DataSubmissionService>();
        services.AddScoped<DiagnosticsProbeService>();
        services.AddScoped<OrderApplicationService>();
        services.AddScoped<CustomerService>();

        services.AddScoped<InventoryManager>();
        services.AddScoped<OrderConsistencyManager>();
        services.AddScoped<OrderStateManager>();
        services.AddScoped<PaymentManager>();
        services.AddScoped<PricingManager>();
        services.AddScoped<FulfillmentManager>();

        services.AddSingleton<ICustomerRepository, InMemoryCustomerRepository>();
        services.AddSingleton<IInventoryRepository, InMemoryInventoryRepository>();
        services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
        services.AddSingleton<IPricingRepository, InMemoryPricingRepository>();
        services.AddSingleton<IDataSubmissionRepository, InMemoryDataSubmissionRepository>();

        services.AddScoped<IPaymentProvider, PaymentGatewayProvider>();
        services.AddScoped<IShippingProvider, ShippingProvider>();

        services.AddScoped<IPaymentStrategy, CreditCardPaymentStrategy>();
        services.AddScoped<IPaymentStrategy, InvoicePaymentStrategy>();
        services.AddScoped<IPaymentStrategy, WalletPaymentStrategy>();
        services.AddScoped<IPaymentStrategyResolver, PaymentStrategyResolver>();
        services.AddScoped<RetryPolicy>();

        services.AddSingleton<AsyncPitfallService>();

        return services;
    }
}
