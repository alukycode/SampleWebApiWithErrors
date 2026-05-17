public sealed class HealthCheckService(TimeProvider timeProvider)
{
    public HealthCheckResponse Check() =>
        new("Success", "Application service layer responded.", timeProvider.GetUtcNow());
}

public sealed class DataSubmissionService(
    IDataSubmissionRepository repository,
    TimeProvider timeProvider
)
{
    public async Task<DataSubmissionResponse> SubmitAsync(
        DataSubmissionRequest request,
        CancellationToken cancellationToken
    )
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors.Add("Name is required.");
        }

        if (request.Name?.Length > 80)
        {
            errors.Add("Name must be 80 characters or fewer.");
        }

        if (errors.Count > 0)
        {
            throw new InputValidationException(errors);
        }

        var submission = new DataSubmissionResponse(
            Guid.NewGuid(),
            request.Name!.Trim(),
            string.IsNullOrWhiteSpace(request.Source) ? "api" : request.Source.Trim(),
            string.IsNullOrWhiteSpace(request.CorrelationId)
                ? Guid.NewGuid().ToString("N")
                : request.CorrelationId.Trim(),
            timeProvider.GetUtcNow()
        );

        await repository.SaveAsync(submission, cancellationToken);
        return submission;
    }
}

public sealed class DiagnosticsProbeService(TimeProvider timeProvider)
{
    public void ThrowUnhandledProbe()
    {
        var operationId = $"probe-{timeProvider.GetUtcNow():yyyyMMddHHmmss}";
        throw new InvalidOperationException(
            $"Application diagnostics probe '{operationId}' failed while publishing telemetry."
        );
    }
}
