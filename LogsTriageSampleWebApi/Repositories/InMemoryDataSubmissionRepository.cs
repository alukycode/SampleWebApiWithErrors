using System.Collections.Concurrent;

public sealed class InMemoryDataSubmissionRepository : IDataSubmissionRepository
{
    private readonly ConcurrentDictionary<Guid, DataSubmissionResponse> submissions = new();

    public Task SaveAsync(DataSubmissionResponse submission, CancellationToken cancellationToken)
    {
        if (string.Equals(submission.Source, "repository-failure", StringComparison.OrdinalIgnoreCase))
        {
            throw new RepositoryAccessException(
                "Submission repository failed while saving the incoming payload.",
                new IOException("Simulated append-only log write failure.")
            );
        }

        submissions[submission.Id] = submission;
        return Task.CompletedTask;
    }
}
