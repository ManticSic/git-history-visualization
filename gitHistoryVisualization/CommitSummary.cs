using Semver;


namespace gitHistoryVisualization;


public struct CommitSummary
{
    public CommitSummary(DateTimeOffset when, int linesAdded, int linesRemoved, bool isRootCommit, string? tag)
    {
        When         = when.Date;
        LinesAdded   = linesAdded;
        LinesRemoved = linesRemoved;
        IsRootCommit = isRootCommit;
        Release      = GetReleaseType(tag);
    }

    public DateTime    When         { get; }
    public int         LinesAdded   { get; }
    public int         LinesRemoved { get; }
    public bool        IsRootCommit { get; }
    public ReleaseType Release      { get; }

    private ReleaseType GetReleaseType(string? versionString)
    {
        if (string.IsNullOrWhiteSpace(versionString))
        {
            return ReleaseType.None;
        }

        versionString = versionString.Remove(0, 1);

        if (!SemVersion.TryParse(versionString, out SemVersion version))
        {
            return ReleaseType.None;
        }

        if (version.Minor == 0 && version.Patch == 0)
        {
            return ReleaseType.Major;
        }

        if (version.Patch == 0)
        {
            return ReleaseType.Minor;
        }

        return ReleaseType.Patch;
    }


    public enum ReleaseType
    {
        None,
        Major,
        Minor,
        Patch
    }
}
