using System.Drawing;


namespace gitHistoryVisualization;


public class DrawUtil
{
    public const  int RefCanvasSize   = 2800;
    private const int RefCircleBorder = 1;

    private static CircleRadiusBoundary RefCircleRadiusBoundry1 = new(100, 4);
    private static CircleRadiusBoundary RefCircleRadiusBoundry2 = new(500, 8);
    private static CircleRadiusBoundary RefCircleRadiusBoundry3 = new(2500, 20);
    private static CircleRadiusBoundary RefCircleRadiusBoundry4 = new(12500, 45);
    private static CircleRadiusBoundary RefCircleRadiusBoundry5 = new(25000, 50);


    private readonly Color                _backgroundColor = Color.FromArgb(255, 20, 28, 48);
    private readonly Color                _primaryColor    = Color.FromArgb(255, 144, 160, 204);
    private readonly Color                _secondaryColor  = Color.FromArgb(255, 10, 16, 36);
    private readonly Color                _highlightColor  = Color.FromArgb(255, 183, 88, 70);
    private readonly float                _circleBorder;
    private readonly CircleRadiusBoundary _circleRadiusBoundary1;
    private readonly CircleRadiusBoundary _circleRadiusBoundary2;
    private readonly CircleRadiusBoundary _circleRadiusBoundary3;
    private readonly CircleRadiusBoundary _circleRadiusBoundary4;
    private readonly CircleRadiusBoundary _circleRadiusBoundary5;


    private readonly Func<int, float> _radiusFunctionLower500;
    private readonly Func<int, float> _radiusFunctionLower2500;
    private readonly Func<int, float> _radiusFunctionLower12500;
    private readonly Func<int, float> _radiusFunctionLower25000;


    public DrawUtil(int canvasSize)
    {
        float factor = CalculateSizeFactor(canvasSize);

        _circleBorder = RefCircleBorder * factor;

        _circleRadiusBoundary1 = new(RefCircleRadiusBoundry1.Limit, RefCircleRadiusBoundry1.Radius * factor);
        _circleRadiusBoundary2 = new(RefCircleRadiusBoundry2.Limit, RefCircleRadiusBoundry2.Radius * factor);
        _circleRadiusBoundary3 = new(RefCircleRadiusBoundry3.Limit, RefCircleRadiusBoundry3.Radius * factor);
        _circleRadiusBoundary4 = new(RefCircleRadiusBoundry4.Limit, RefCircleRadiusBoundry4.Radius * factor);
        _circleRadiusBoundary5 = new(RefCircleRadiusBoundry5.Limit, RefCircleRadiusBoundry5.Radius * factor);

        _radiusFunctionLower500 =
            GenerateCircleCalculation(_circleRadiusBoundary1, _circleRadiusBoundary2);
        _radiusFunctionLower2500 =
            GenerateCircleCalculation(_circleRadiusBoundary2, _circleRadiusBoundary3);
        _radiusFunctionLower12500 =
            GenerateCircleCalculation(_circleRadiusBoundary3, _circleRadiusBoundary4);
        _radiusFunctionLower25000 =
            GenerateCircleCalculation(_circleRadiusBoundary4, _circleRadiusBoundary5);
    }

    public Color BackgroundColor => _backgroundColor;

    public void DrawDateSummary(Graphics graphics, DateSummary dateSummary, float x, float y)
    {
        int linesChanged = dateSummary.LinesAdded - dateSummary.LinesRemoved;

        float radius;
        Color ellipseColor;
        Color borderColor;

        if (dateSummary.LinesAdded >= dateSummary.LinesRemoved)
        {
            ellipseColor = _primaryColor;
            borderColor  = _secondaryColor;
            radius       = CalculateRadius(dateSummary.LinesAdded);
        }
        else
        {
            ellipseColor = _backgroundColor;
            borderColor  = _primaryColor;
            radius       = CalculateRadius(dateSummary.LinesRemoved);
        }

        if (dateSummary.ContainsRootCommit)
        {
            ellipseColor = _highlightColor;
            borderColor  = _secondaryColor;
        }

        using SolidBrush fillBrush = new(ellipseColor);
        graphics.FillEllipse(fillBrush, x - radius, y - radius, 2 * radius, 2 * radius);

        using Pen borderPen = new(borderColor, _circleBorder);
        graphics.DrawEllipse(borderPen, x - radius, y - radius, 2 * radius, 2 * radius);
    }

    private float CalculateRadius(int lines)
    {
        if (lines < _circleRadiusBoundary1.Limit)
        {
            return _circleRadiusBoundary1.Radius;
        }

        if (lines < _circleRadiusBoundary2.Limit)
        {
            return _radiusFunctionLower500(lines);
        }

        if (lines < _circleRadiusBoundary3.Limit)
        {
            return _radiusFunctionLower2500(lines);
        }

        if (lines < _circleRadiusBoundary4.Limit)
        {
            _radiusFunctionLower12500(lines);
        }

        if (lines < _circleRadiusBoundary5.Limit)
        {
            _radiusFunctionLower25000(lines);
        }

        return _circleRadiusBoundary5.Radius;
    }


    private static float CalculateSizeFactor(int canvasSize)
    {
        return (float)RefCanvasSize / canvasSize;
    }

    private static Func<int, float> GenerateCircleCalculation(CircleRadiusBoundary point1, CircleRadiusBoundary point2)
    {
        (int X, float Y) p1 = (X: point1.Limit, Y: point1.Radius);
        (int X, float Y) p2 = (X: point2.Limit, Y: point2.Radius);

        float slope     = (p2.Y - p1.Y) / (p2.X - p1.X);
        float intercept = p1.Y - slope * p1.X;

        return lines => slope * lines + intercept;
    }


    private struct CircleRadiusBoundary
    {
        public CircleRadiusBoundary(int limit, float radius)
        {
            Limit  = limit;
            Radius = radius;
        }

        public int   Limit  { get; }
        public float Radius { get; }
    }
}
