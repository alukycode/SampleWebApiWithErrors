using Microsoft.EntityFrameworkCore;

public sealed class EfMigrationNotAppliedIssueHandler(IssueDbContext db)
{
    public async Task<IResult> Handle()
    {
        var count = await db.IssueWidgets.CountAsync();
        return Results.Ok(new { Count = count });
    }
}
