﻿using System.Drawing;
using System.Drawing.Drawing2D;


namespace gitHistoryVisualization;


public class DrawUtil
{
    public const  int RefCanvasSize = 2800;
    private const int RefBorder     = 1;

    private static List<CircleRadiusRange> RefCircleRadiusBoundries = new()
    {
        new CircleRadiusRange(100, 4, 500, 8),
        new CircleRadiusRange(500, 8, 2500, 12),
        new CircleRadiusRange(2500, 12, 12500, 20),
        new CircleRadiusRange(12500, 20, 25000, 48),
    };

    private readonly Color _backgroundColor = Color.FromArgb(255, 20, 28, 48);
    private readonly Color _primaryColor    = Color.FromArgb(255, 144, 160, 204);
    private readonly Color _secondaryColor  = Color.FromArgb(255, 10, 16, 36);
    private readonly Color _highlightColor  = Color.FromArgb(255, 183, 88, 70);
    private readonly float _border;

    private readonly List<CircleRadiusRange> _circleRadiusBoundaries;


    public DrawUtil(int canvasSize)
    {
        float factor = CalculateSizeFactor(canvasSize);

        _border = RefBorder * factor;

        _circleRadiusBoundaries = RefCircleRadiusBoundries
                                  .Select(boundary => new CircleRadiusRange(boundary.LeftLimit, boundary.LeftRadius * factor,
                                                                            boundary.RightLimit, boundary.RightRadius * factor))
                                  .OrderBy(boundary => boundary.LeftLimit)
                                  .ToList();
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

        using Pen borderPen = new(borderColor, _border);
        graphics.DrawEllipse(borderPen, x - radius, y - radius, 2 * radius, 2 * radius);
    }

    public void DrawIsoscelesTriangle(Graphics g, float baseX, float baseY, float baseWidth, float height, float angle = 0)
    {
        // Calculate the triangle's points
        PointF[] trianglePoints =
        {
            new(baseX, baseY),
            new(baseX + baseWidth / 2, baseY - height),
            new(baseX + baseWidth, baseY)
        };

        // Create rotation matrix
        Matrix matrix = new();
        matrix.RotateAt(angle, new PointF(baseX + baseWidth / 2, baseY - height / 2));

        // Apply rotation
        matrix.TransformPoints(trianglePoints);

        // Draw the filled triangle
        using SolidBrush brush = new(_highlightColor);
        g.FillPolygon(brush, trianglePoints);

        // Add border to the Triangle
        using Pen borderPen = new(_backgroundColor, _border);
        g.DrawPolygon(borderPen, trianglePoints);
    }

    private float CalculateRadius(int lines)
    {
        if (lines <= _circleRadiusBoundaries.First().LeftLimit)
        {
            return _circleRadiusBoundaries.First().LeftRadius;
        }

        if (lines >= _circleRadiusBoundaries.Last().RightLimit)
        {
            return _circleRadiusBoundaries.Last().RightRadius;
        }

        return _circleRadiusBoundaries.First(range => range.IsInRange(lines)).CalcCircleRadius(lines);
    }

    private static float CalculateSizeFactor(int canvasSize)
    {
        return (float)RefCanvasSize / canvasSize;
    }


    private readonly struct CircleRadiusRange
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
