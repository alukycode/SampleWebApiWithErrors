public sealed class NullableUnexpectedlyNullIssueHandler
{
    public IResult Handle()
    {
        DateTime? expiresAt = null;
        return Results.Ok(new { expiresAt = expiresAt.GetValueOrDefault() });
    }
}
