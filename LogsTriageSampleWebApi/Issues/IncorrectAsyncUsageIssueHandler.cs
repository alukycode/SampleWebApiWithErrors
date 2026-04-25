public sealed class IncorrectAsyncUsageIssueHandler(AsyncPitfallService service)
{
    public IResult Handle()
    {
        var backgroundWork = service.RunAsync();
        if (!backgroundWork.IsCompleted)
        {
            throw new InvalidOperationException(
                "A background operation was started but never awaited before the response path continued."
            );
        }

        return Results.Ok(new { Status = "Unexpected success" });
    }
}
