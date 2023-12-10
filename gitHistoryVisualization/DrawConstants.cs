using System.Drawing;


namespace gitHistoryVisualization;


public class DrawConstants
{
    #region reference values

    public const  int RefCanvasSize           = 2800;
    private const int RefBorder               = 1;
    private const int RefTriangleDisplacement = 2;
    private const int RefTriangleSize         = 12;

    private static List<CircleRadiusRange> RefCircleRadiusBoundries = new()
    {
        new CircleRadiusRange(100, 2, 500, 8),
        new CircleRadiusRange(500, 8, 2500, 12),
        new CircleRadiusRange(2500, 12, 12500, 20),
        new CircleRadiusRange(12500, 20, 25000, 48),
    };

    #endregion

    public DrawConstants(int canvasSize)
    {
        float factor = (float)RefCanvasSize / canvasSize;

        Border               = RefBorder * factor;
        TriangleDisplacement = RefTriangleDisplacement * factor;
        TriangleSize         = RefTriangleSize * factor;

        BackgroundColor = Color.FromArgb(255, 20, 28, 48);
        PrimaryColor    = Color.FromArgb(255, 144, 160, 204);
        SecondaryColor  = Color.FromArgb(255, 10, 16, 36);
        HighlightColor  = Color.FromArgb(255, 183, 88, 70);

        CircleRadiusBoundaries = RefCircleRadiusBoundries
                                 .Select(boundary => new CircleRadiusRange(boundary.LeftLimit, boundary.LeftRadius * factor,
                                                                           boundary.RightLimit, boundary.RightRadius * factor))
                                 .OrderBy(boundary => boundary.LeftLimit)
                                 .ToList();
    }

    public Color                   BackgroundColor        { get; }
    public Color                   PrimaryColor           { get; }
    public Color                   SecondaryColor         { get; }
    public Color                   HighlightColor         { get; }
    public float                   Border                 { get; }
    public float                   TriangleDisplacement   { get; }
    public float                   TriangleSize           { get; }
    public List<CircleRadiusRange> CircleRadiusBoundaries { get; }


    public readonly struct CircleRadiusRange
    {
        private readonly float _slope;
        private readonly float _intercept;

        public CircleRadiusRange(int leftLimit, float leftRadius, int rightLimit, float rightRadius)
        {
            LeftLimit   = leftLimit;
            LeftRadius  = leftRadius;
            RightLimit  = rightLimit;
            RightRadius = rightRadius;

            _slope     = (RightRadius - LeftRadius) / (RightLimit - LeftLimit);
            _intercept = leftRadius - _slope * leftLimit;
        }

        public int   LeftLimit   { get; }
        public float LeftRadius  { get; }
        public int   RightLimit  { get; }
        public float RightRadius { get; }

        public float CalcCircleRadius(float lines)
        {
            return _slope * lines + _intercept;
        }

        public bool IsInRange(int lines)
        {
            return LeftLimit <= lines && lines <= RightLimit;
        }
    }
}
