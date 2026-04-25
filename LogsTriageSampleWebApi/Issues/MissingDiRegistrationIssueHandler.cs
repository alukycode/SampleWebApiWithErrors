using Microsoft.Extensions.DependencyInjection;

public sealed class MissingDiRegistrationIssueHandler(IServiceProvider services)
{
    public IResult Handle()
    {
        var dependency = services.GetRequiredService<IUnregisteredDependency>();
        return Results.Ok(new { dependency.Name });
    }
}
