using System.Drawing;
using System.Drawing.Drawing2D;


namespace gitHistoryVisualization;


public class DrawUtil
{
    private readonly DrawConstants _constants;

    public DrawUtil(DrawConstants constants)
    {
        _constants = constants;
    }

    public Color BackgroundColor => _constants.BackgroundColor;

    public void DrawDateSummary(Graphics graphics, DateSummary dateSummary, ArchimedeanSpiral.Point point)
    {
        float radius;
        Color ellipseColor;
        Color borderColor;

        if (dateSummary.LinesAdded >= dateSummary.LinesRemoved)
        {
            ellipseColor = _constants.PrimaryColor;
            borderColor  = _constants.SecondaryColor;
            radius       = CalculateRadius(dateSummary.LinesAdded);
        }
        else
        {
            ellipseColor = _constants.BackgroundColor;
            borderColor  = _constants.PrimaryColor;
            radius       = CalculateRadius(dateSummary.LinesRemoved);
        }

        if (dateSummary.ContainsRootCommit)
        {
            ellipseColor = _constants.HighlightColor;
            borderColor  = _constants.SecondaryColor;
        }


        using SolidBrush fillBrush = new(ellipseColor);
        graphics.FillEllipse(fillBrush, point.X - radius, point.Y - radius, 2 * radius, 2 * radius);

        using Pen borderPen = new(borderColor, _constants.Border);
        graphics.DrawEllipse(borderPen, point.X - radius, point.Y - radius, 2 * radius, 2 * radius);

        if (dateSummary.Release is CommitSummary.ReleaseType.Major or CommitSummary.ReleaseType.Minor)
        {
            float angle = 365f / 360f * dateSummary.When.DayOfYear;

            DrawIsoscelesTriangle(graphics, point.X, point.Y, _constants.TriangleSize, angle, radius);
        }
    }

    public void DrawIsoscelesTriangle(Graphics g, float centerX, float centerY, float baseSize, float angle, float triangleDisplacement)
    {
        // Calculate displacement
        float totalDisplacement = triangleDisplacement + _constants.TriangleSize / 2 + _constants.TriangleDisplacement;
        float displacementX     = totalDisplacement * -1 * (float)Math.Sin(angle * Math.PI / 180);
        float displacementY     = totalDisplacement * (float)Math.Cos(angle * Math.PI / 180);

        centerX += displacementX;
        centerY += displacementY;

        // Calculate the triangle's points
        PointF[] trianglePoints =
        {
            new(centerX - baseSize / 2, centerY + baseSize / 2),
            new(centerX, centerY - baseSize / 2),
            new(centerX + baseSize / 2, centerY + baseSize / 2)
        };

        // Create rotation matrix
        Matrix matrix = new();
        matrix.RotateAt(angle, new PointF(centerX, centerY));

        // Apply rotation
        matrix.TransformPoints(trianglePoints);

        // Draw the filled triangle
        using SolidBrush brush = new(_constants.HighlightColor);
        g.FillPolygon(brush, trianglePoints);

        // Add border to the Triangle
        using Pen borderPen = new(_constants.BackgroundColor, _constants.Border);
        g.DrawPolygon(borderPen, trianglePoints);
    }

    private float CalculateRadius(int lines)
    {
        if (lines <= _constants.CircleRadiusBoundaries.First().LeftLimit)
        {
            return _constants.CircleRadiusBoundaries.First().LeftRadius;
        }

        if (lines >= _constants.CircleRadiusBoundaries.Last().RightLimit)
        {
            return _constants.CircleRadiusBoundaries.Last().RightRadius;
        }

        return _constants.CircleRadiusBoundaries.First(range => range.IsInRange(lines)).CalcCircleRadius(lines);
    }
}
