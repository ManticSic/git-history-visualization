using CommandLine;


namespace gitHistoryVisualization;


public class Options
{
    [Option('i', "input", Required = true, HelpText = "Path to git repository.")]
    public string PathToRepository { get; set; }

    [Option('o', "output", Required = true, HelpText = "Path to save the png file.")]
    public string OutputPath { get; set; }

    [Option('s', "size", Required = false, Default = DrawConstants.RefCanvasSize, HelpText = "Size of the image (square).")]
    public int Size { get; set; }
}
