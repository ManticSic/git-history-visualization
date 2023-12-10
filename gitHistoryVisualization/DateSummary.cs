namespace gitHistoryVisualization;


public struct DateSummary
{
    private readonly IEnumerable<CommitSummary> _commits;
    private readonly Lazy<int>                  _lazyLinesAdded;
    private readonly Lazy<int>                  _lazyLinesRemoved;

    public DateSummary(DateTime dateTime, List<CommitSummary> sourceCommits, bool containsRootCommit)
    {
        When               = dateTime;
        ContainsRootCommit = containsRootCommit;
        Release            = sourceCommits.Max(summary => summary.Release);

        _commits          = sourceCommits;
        _lazyLinesAdded   = CreateLazyLinesAdded();
        _lazyLinesRemoved = CreateLazyLinesRemoved();
    }

    public DateTime                  When               { get; }
    public bool                      ContainsRootCommit { get; }
    public CommitSummary.ReleaseType Release            { get; }
    public int                       LinesAdded         => _lazyLinesAdded.Value;
    public int                       LinesRemoved       => _lazyLinesRemoved.Value;


    private Lazy<int> CreateLazyLinesAdded()
    {
        return new Lazy<int>(LinesAddedFactory);
    }

    private Lazy<int> CreateLazyLinesRemoved()
    {
        return new Lazy<int>(LinesRemovedFactory);
    }

    private int LinesAddedFactory()
    {
        return _commits.Sum(commit => commit.LinesAdded);
    }

    private int LinesRemovedFactory()
    {
        return _commits.Sum(commit => commit.LinesRemoved);
    }
}
