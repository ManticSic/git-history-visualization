﻿using System.Diagnostics;
using System.Drawing.Imaging;
using CommandLine;
using LibGit2Sharp;
using System.Text.RegularExpressions;


namespace gitHistoryVisualization;


internal class Program
{
    private static CommitUtil _commitUtill;
    private static Regex      _tagPattern = new Regex(@"^v\d+.\d+.\d+$", RegexOptions.Compiled);

    private static void Main(string[] args)
    {
        Stopwatch swTotal = Stopwatch.StartNew();

        // Parse arguments
        Options options = Parser.Default.ParseArguments<Options>(args).Value;

        _commitUtill = new CommitUtil();

        using Repository repository = new(options.PathToRepository);

        List<DateSummary> commits = GetDateSummaries(repository);

        DateTime startDate = commits.Min(summary => summary.When);
        DateTime endDate   = commits.Max(summary => summary.When);

        Console.WriteLine($"Total entries to process: {commits.Count}");
        Stopwatch swDraw = Stopwatch.StartNew();

        ArchimedeanSpiral spiral = new(options.Size, 365, startDate, endDate);
        using Canvas      canvas = new(options.Size, 50, spiral);

        foreach (DateSummary dateSummary in commits)
        {
            canvas.DrawCommit(dateSummary);
        }

        // save image
        canvas.Save(options.OutputPath, ImageFormat.Png);

        swDraw.Stop();
        swTotal.Stop();
        Console.WriteLine($"Image completely drawn after {swDraw.Elapsed.TotalSeconds} seconds.");
        Console.WriteLine($"Total: {swTotal.Elapsed.TotalSeconds} seconds");
    }

    private static Dictionary<string, Tag> GetReleases(Repository repository)
    {
        return repository.Tags
                         .Where(tag => _tagPattern.IsMatch(tag.FriendlyName))
                         .ToDictionary(tag => tag.Target.Sha, tag => tag);
    }

    private static List<DateSummary> GetDateSummaries(Repository repository)
    {
        Console.WriteLine("Start processing commits.");
        Stopwatch swProcessCommits = Stopwatch.StartNew();

        Dictionary<string, Tag> releases = GetReleases(repository);

        List<DateSummary> commits = repository.Commits
                                              .AsParallel()
                                              .WithDegreeOfParallelism(Environment.ProcessorCount > 1 ? Environment.ProcessorCount - 1 : 1)
                                              .Select(commit => _commitUtill.CommitToCommitSummary(commit, repository, releases))
                                              .GroupBy(summary => summary.When)
                                              .Select(grouping => _commitUtill.CreateDateSummary(grouping))
                                              .OrderBy(summary => summary.When)
                                              .ToList();
        swProcessCommits.Stop();
        Console.WriteLine($"Finished processing of commits after {swProcessCommits.Elapsed.TotalSeconds} seconds.");
        return commits;
    }
}
