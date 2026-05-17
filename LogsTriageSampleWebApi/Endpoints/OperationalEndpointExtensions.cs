using Microsoft.AspNetCore.Mvc;

public static class OperationalEndpointExtensions
{
    public static IEndpointRouteBuilder MapOperationalEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var api = endpoints.MapGroup("/api")
            .WithTags("Operational");

        api.MapGet(
                "/test",
                ([FromServices] HealthCheckHandler handler) => handler.Handle()
            )
            .WithSummary("Runs a simple test")
            .WithDescription("Returns a success message from the application service layer.");

        api.MapPost(
                "/data",
                (
                    [FromServices] SubmitDataHandler handler,
                    DataSubmissionRequest request,
                    CancellationToken cancellationToken
                ) => handler.HandleAsync(request, cancellationToken)
            )
            .WithSummary("Submits sample data")
            .WithDescription("Accepts a JSON payload and stores it through the submission service.");

        api.MapGet(
                "/error",
                ([FromServices] DiagnosticsProbeHandler handler) => handler.Handle()
            )
            .WithSummary("Throws a test exception")
            .WithDescription("Generates an unhandled exception through the diagnostics service layer.");

        return endpoints;
    }
}
