public static class IssueCatalog
{
    public static readonly object[] All =
    [
        new
        {
            Id = "automapper-missing-map",
            Path = "/api/issues/automapper-missing-map",
            Failure = "AutoMapperMappingException: Missing type map configuration or unsupported mapping",
        },
        new
        {
            Id = "missing-di-registration",
            Path = "/api/issues/missing-di-registration",
            Failure = "InvalidOperationException: Unable to resolve service for type ... while attempting to activate ...",
        },
        new
        {
            Id = "missing-configuration",
            Path = "/api/issues/missing-configuration",
            Failure = "InvalidOperationException: Missing required configuration key ...",
        },
        new
        {
            Id = "ef-migration-not-applied",
            Path = "/api/issues/ef-migration-not-applied",
            Failure = "SQLite Error 1: no such table: IssueWidgets",
        },
        new
        {
            Id = "nullable-unexpectedly-null",
            Path = "/api/issues/nullable-unexpectedly-null",
            Failure = "InvalidOperationException: Nullable object must have a value",
        },
        new
        {
            Id = "enum-parse-mismatch",
            Path = "/api/issues/enum-parse-mismatch",
            Failure = "JsonException when enum values do not match expected strings",
        },
        new
        {
            Id = "incorrect-async-usage",
            Path = "/api/issues/incorrect-async-usage",
            Failure = "InvalidOperationException because background work was started without awaiting it",
        },
    ];
}
