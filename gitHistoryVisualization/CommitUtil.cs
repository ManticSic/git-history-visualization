using LibGit2Sharp;


namespace gitHistoryVisualization;


public class CommitUtil
{
    public Commit GetRootCommit(Repository repo)
    {
        Commit commit = repo.Head.Tip;

        while (commit.Parents.Any())
        {
            commit = commit.Parents.First();
        }

        return commit;
    }

    public CommitSummary CommitToCommitSummary(Commit commit, Repository repository)
    {
        Tree  commitTree       = commit.Tree;
        Tree? parentCommitTree = null;

        bool isRootCommit = true;

        // handle root commit
        if (commit.Parents.Any())
        {
            isRootCommit     = false;
            parentCommitTree = commit.Parents.First().Tree;
        }

        Patch patch = repository.Diff.Compare<Patch>(parentCommitTree, commitTree);

        return new CommitSummary(commit.Author.When, patch.LinesAdded, patch.LinesDeleted, isRootCommit);
    }
}
