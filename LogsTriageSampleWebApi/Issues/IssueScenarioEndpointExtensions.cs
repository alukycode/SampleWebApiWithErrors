using Microsoft.AspNetCore.Mvc;

public static class IssueScenarioEndpointExtensions
{
    public static IServiceCollection AddIssueScenarioHandlers(this IServiceCollection services)
    {
        services.AddTransient<AutoMapperMissingMapIssueHandler>();
        services.AddTransient<MissingDiRegistrationIssueHandler>();
        services.AddTransient<MissingConfigurationIssueHandler>();
        services.AddTransient<EfMigrationNotAppliedIssueHandler>();
        services.AddTransient<NullableUnexpectedlyNullIssueHandler>();
        services.AddTransient<EnumParseMismatchIssueHandler>();
        services.AddTransient<IncorrectAsyncUsageIssueHandler>();
        return services;
    }

    public static IEndpointRouteBuilder MapIssueScenarioEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var issues = endpoints.MapGroup("/api/issues")
            .WithOpenApi()
            .WithTags("Issue scenarios");

        issues.MapGet("/", () => Results.Ok(IssueCatalog.All))
            .WithSummary("Lists the available runtime issue scenarios")
            .WithDescription("Returns the issue endpoints that intentionally reproduce common runtime failures.");

        issues.MapPost(
                "/automapper-missing-map",
                ([FromServices] AutoMapperMissingMapIssueHandler handler) => handler.Handle()
            )
            .WithSummary("Triggers a missing AutoMapper mapping error")
            .WithDescription("Reproduces AutoMapperMappingException caused by missing type map configuration.");

        issues.MapGet(
                "/missing-di-registration",
                ([FromServices] MissingDiRegistrationIssueHandler handler) => handler.Handle()
            )
            .WithSummary("Triggers a missing dependency injection registration")
            .WithDescription("Reproduces DI activation failure for an unregistered service.");

        issues.MapGet(
                "/missing-configuration",
                ([FromServices] MissingConfigurationIssueHandler handler) => handler.Handle()
            )
            .WithSummary("Triggers a missing configuration value failure")
            .WithDescription("Reproduces failed service activation when a required appsettings key is absent.");

        issues.MapGet(
                "/ef-migration-not-applied",
                ([FromServices] EfMigrationNotAppliedIssueHandler handler) => handler.Handle()
            )
            .WithSummary("Triggers an EF schema mismatch")
            .WithDescription("Reproduces the database failure you see when the expected EF migration was not applied.");

        issues.MapGet(
                "/nullable-unexpectedly-null",
                ([FromServices] NullableUnexpectedlyNullIssueHandler handler) => handler.Handle()
            )
            .WithSummary("Triggers a nullable value failure")
            .WithDescription("Reproduces InvalidOperationException when a nullable value is unexpectedly null.");

        issues.MapPost(
                "/enum-parse-mismatch",
                ([FromServices] EnumParseMismatchIssueHandler handler, HttpRequest request) => handler.HandleAsync(request)
            )
            .WithSummary("Triggers an enum parsing or deserialization failure")
            .WithDescription("Reproduces JsonException when an unexpected enum string is sent in the request body.");

        issues.MapGet(
                "/incorrect-async-usage",
                ([FromServices] IncorrectAsyncUsageIssueHandler handler) => handler.Handle()
            )
            .WithSummary("Triggers an incorrect async usage failure")
            .WithDescription("Reproduces a fire-and-forget async mistake where work is started but not awaited.");

        return endpoints;
    }
}
