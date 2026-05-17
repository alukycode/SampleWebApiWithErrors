public sealed class HealthCheckHandler(HealthCheckService service)
{
    public IResult Handle() => Results.Ok(service.Check());
}

public sealed class SubmitDataHandler(DataSubmissionService service)
{
    public async Task<IResult> HandleAsync(
        DataSubmissionRequest request,
        CancellationToken cancellationToken
    )
    {
        var response = await service.SubmitAsync(request, cancellationToken);
        return Results.Ok(response);
    }
}

public sealed class DiagnosticsProbeHandler(DiagnosticsProbeService service)
{
    public IResult Handle()
    {
        service.ThrowUnhandledProbe();
        return Results.Ok();
    }
}
