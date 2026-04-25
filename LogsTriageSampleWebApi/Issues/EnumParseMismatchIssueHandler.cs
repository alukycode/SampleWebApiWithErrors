public sealed class EnumParseMismatchIssueHandler
{
    public async Task<IResult> HandleAsync(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body);
        var payload = await reader.ReadToEndAsync();
        var parsed = IssueScenarioJson.DeserializeEnumPayload(payload);
        return Results.Ok(parsed);
    }
}
