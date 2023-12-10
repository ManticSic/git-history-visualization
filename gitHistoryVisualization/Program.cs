using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using CommandLine;
using LibGit2Sharp;


namespace gitHistoryVisualization;


internal class Program
{
    private static DrawUtil   _drawUtil;
    private static CommitUtil _commitUtill;

    private static void Main(string[] args)
    {
        Stopwatch swTotal = Stopwatch.StartNew();

        // Parse arguments
        Options options = Parser.Default.ParseArguments<Options>(args).Value;

        _commitUtill = new CommitUtil();
        _drawUtil    = new DrawUtil(options.Size);

        using Repository repository = new(options.PathToRepository);

        Console.WriteLine("Start processing commits.");

        Stopwatch swProcessCommits = Stopwatch.StartNew();
        List<DateSummary> commits = repository.Commits
                                              .AsParallel()
                                              .WithDegreeOfParallelism(Environment.ProcessorCount > 1 ? Environment.ProcessorCount - 1 : 1)
                                              .Select(commit => _commitUtill.CommitToCommitSummary(commit, repository))
                                              .GroupBy(summary => summary.When)
                                              .Select(grouping => new DateSummary(grouping.Key, grouping, grouping.Any(summary => summary.IsRootCommit)))
                                              .OrderBy(summary => summary.When)
                                              .ToList();
        swProcessCommits.Stop();
        Console.WriteLine($"Finished processing of commits after {swProcessCommits.Elapsed.TotalSeconds} seconds.");

        DateTime startDate = commits.Min(summary => summary.When);
        DateTime endDate   = commits.Max(summary => summary.When);


        ArchimedeanSpiral spiral = new(options.Size, 365, startDate, endDate);

        Console.WriteLine($"Total entries to process: {commits.Count}");
        Stopwatch swDraw = Stopwatch.StartNew();

        using Canvas canvas = new(options.Size, 50);

        canvas.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
        canvas.Graphics.Clear(_drawUtil.BackgroundColor);

        foreach (DateSummary dateSummary in commits)
        {
            float angle = spiral.StartAngle + CalculateDaysSinceStart(dateSummary, startDate) * spiral.Increment;
            float radius = (float)(spiral.InnerDiameter +
                                   (spiral.OuterDiameter - spiral.InnerDiameter) * (angle - spiral.StartAngle) / (spiral.Turns * 2 * Math.PI));

            float x = canvas.CenterX + radius * (float)Math.Cos(angle);
            float y = canvas.CenterY + radius * (float)Math.Sin(angle);

            _drawUtil.DrawDateSummary(canvas.Graphics, dateSummary, x, y);
        }

        // save image
        canvas.Bitmap.Save(options.OutputPath, ImageFormat.Png);

        swDraw.Stop();
        swTotal.Stop();
        Console.WriteLine($"Image completely drawn after {swDraw.Elapsed.TotalSeconds} seconds.");
        Console.WriteLine($"Total: {swTotal.Elapsed.TotalSeconds} seconds");
    }

    private static int CalculateDaysSinceStart(DateSummary dateSummary, DateTime startDate)
    {
        TimeSpan span = dateSummary.When - startDate;
        return span.Days;
    }
}
