using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

public sealed class IssueDbContext(DbContextOptions<IssueDbContext> options) : DbContext(options)
{
    public DbSet<IssueWidget> IssueWidgets => Set<IssueWidget>();
}

public sealed class IssueWidget
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class AsyncPitfallService
{
    public async Task RunAsync()
    {
        await Task.Delay(250);
        throw new InvalidOperationException("The detached task failed after the request moved on.");
    }
}

public interface IUnregisteredDependency
{
    string Name { get; }
}

public sealed record SampleMappingRequest(string Name, int Quantity);

public sealed record SampleMappingResponse(string Name, int Quantity, decimal Total);

public enum DeploymentMode
{
    Development,
    Production,
    Canary,
}

public sealed class EnumPayload
{
    public DeploymentMode Mode { get; init; }
}

public static class IssueScenarioJson
{
    private static readonly JsonSerializerOptions EnumOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public static EnumPayload? DeserializeEnumPayload(string payload) =>
        JsonSerializer.Deserialize<EnumPayload>(payload, EnumOptions);
}
