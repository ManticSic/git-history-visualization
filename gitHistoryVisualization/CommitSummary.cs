namespace gitHistoryVisualization;


public struct CommitSummary
{
    public CommitSummary(DateTimeOffset when, int linesAdded, int linesRemoved, bool isRootCommit)
    {
        When         = when.Date;
        LinesAdded   = linesAdded;
        LinesRemoved = linesRemoved;
        IsRootCommit = isRootCommit;
    }

    public DateTime When         { get; }
    public int      LinesAdded   { get; }
    public int      LinesRemoved { get; }
    public bool     IsRootCommit { get; }
}
