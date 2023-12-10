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

        float outerDiameter = options.Size / 2f;
        float innerDiameter = options.Size / 5f;
        int   imageSize     = options.Size + 100; // add margin
        float centerX       = imageSize / 2f;
        float centerY       = imageSize / 2f;

        DateTime startDate          = commits.Min(summary => summary.When);
        DateTime endDate            = commits.Max(summary => summary.When);
        int      totalYears         = endDate.Year - startDate.Year + 1;
        int      stepsPerRevolution = 365;
        float    increment          = (float)(2 * Math.PI / stepsPerRevolution);
        float    startAngle         = (float)((startDate.DayOfYear / 365f * 360 + 270) * Math.PI / 180);

        Console.WriteLine($"Total entries to process: {commits.Count}");
        Stopwatch swDraw = Stopwatch.StartNew();

        Bitmap bitmap = new(imageSize, imageSize);
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(_drawUtil.BackgroundColor);

            foreach (DateSummary dateSummary in commits)
            {
                float angle  = startAngle + CalculateDaysSinceStart(dateSummary, startDate) * increment;
                float radius = (float)(innerDiameter + (outerDiameter - innerDiameter) * (angle - startAngle) / (totalYears * 2 * Math.PI));

                float x = centerX + radius * (float)Math.Cos(angle);
                float y = centerY + radius * (float)Math.Sin(angle);

                _drawUtil.DrawDateSummary(g, dateSummary, x, y);
            }
        }

        // save image
        bitmap.Save(options.OutputPath, ImageFormat.Png);

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
