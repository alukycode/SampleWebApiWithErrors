public sealed class MissingConfigurationIssueHandler(IConfiguration configuration)
{
    public IResult Handle()
    {
        var connectionString = configuration["IssueScenarios:PrimarySqlConnection"]
            ?? throw new InvalidOperationException(
                "Missing required configuration key 'IssueScenarios:PrimarySqlConnection'."
            );

        return Results.Ok(new { connection = connectionString });
    }
}
