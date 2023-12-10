using System.Drawing;
using System.Drawing.Drawing2D;


namespace gitHistoryVisualization;


public class DrawUtil
{
    public DrawUtil(DrawConstants constants)
    {
        Constants = constants;
    }

    public DrawConstants Constants { get; }

    public void DrawDateSummary(Graphics graphics, DateSummary dateSummary, ArchimedeanSpiral.Point point)
    {
        float radius;
        Color ellipseColor;
        Color borderColor;

        if (dateSummary.LinesAdded >= dateSummary.LinesRemoved)
        {
            ellipseColor = Constants.PrimaryColor;
            borderColor  = Constants.SecondaryColor;
            radius       = CalculateRadius(dateSummary.LinesAdded);
        }
        else
        {
            ellipseColor = Constants.BackgroundColor;
            borderColor  = Constants.PrimaryColor;
            radius       = CalculateRadius(dateSummary.LinesRemoved);
        }

        if (dateSummary.ContainsRootCommit)
        {
            ellipseColor = Constants.HighlightColor;
            borderColor  = Constants.SecondaryColor;
        }


        using SolidBrush fillBrush = new(ellipseColor);
        graphics.FillEllipse(fillBrush, point.X - radius, point.Y - radius, 2 * radius, 2 * radius);

        using Pen borderPen = new(borderColor, Constants.Border);
        graphics.DrawEllipse(borderPen, point.X - radius, point.Y - radius, 2 * radius, 2 * radius);
    }

    public void DrawRelease(Graphics graphics, DateSummary dateSummary, ArchimedeanSpiral.Point point)
    {
        float angle = 365f / 360f * dateSummary.When.DayOfYear;
        DrawIsoscelesTriangle(graphics, point.X, point.Y, Constants.TriangleSize, angle);
    }

    public void DrawIsoscelesTriangle(Graphics g, float centerX, float centerY, float baseSize, float angle)
    {
        // Calculate displacement
        float totalDisplacement = Constants.TriangleSize / 2 + Constants.TriangleDisplacement;
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
        using SolidBrush brush = new(Constants.HighlightColor);
        g.FillPolygon(brush, trianglePoints);

        // Add border to the Triangle
        using Pen borderPen = new(Constants.BackgroundColor, Constants.Border);
        g.DrawPolygon(borderPen, trianglePoints);
    }

    private float CalculateRadius(int lines)
    {
        if (lines <= Constants.CircleRadiusBoundaries.First().LeftLimit)
        {
            return Constants.CircleRadiusBoundaries.First().LeftRadius;
        }

        if (lines >= Constants.CircleRadiusBoundaries.Last().RightLimit)
        {
            return Constants.CircleRadiusBoundaries.Last().RightRadius;
        }

        return Constants.CircleRadiusBoundaries.First(range => range.IsInRange(lines)).CalcCircleRadius(lines);
    }
}
