namespace gitHistoryVisualization;


public struct ArchimedeanSpiral
{
    private readonly DateTime _startDate;
    private readonly float    _center;

    public ArchimedeanSpiral(float size, int stepsPerRevolution, DateTime startDate, DateTime endDate)
    {
        _startDate = startDate;
        _center    = size / 2;

        OuterDiameter = size / 2;
        InnerDiameter = size / 6;
        Increment     = (float)(2 * Math.PI / stepsPerRevolution);
        StartAngle    = (float)((startDate.DayOfYear / 365f * 360 + 270) * Math.PI / 180);
        Turns         = endDate.Year - startDate.Year + 1;
    }

    public float OuterDiameter { get; }
    public float InnerDiameter { get; }
    public float Increment     { get; }
    public float StartAngle    { get; }
    public int   Turns         { get; }

    public Point GetPoint(DateSummary dateSummary)
    {
        float angle  = StartAngle + CalculateDaysSinceStart(dateSummary) * Increment;
        float radius = (float)(InnerDiameter + (OuterDiameter - InnerDiameter) * (angle - StartAngle) / (Turns * 2 * Math.PI));

        float x = _center + radius * (float)Math.Cos(angle);
        float y = _center + radius * (float)Math.Sin(angle);

        return new Point(x, y, angle, radius);
    }

    private int CalculateDaysSinceStart(DateSummary dateSummary)
    {
        TimeSpan span = dateSummary.When - _startDate;
        return span.Days;
    }


    public readonly struct Point
    {
        public Point(float x, float y, float angle, float radius)
        {
            X      = x;
            Y      = y;
            Angle  = angle;
            Radius = radius;
        }

        public float X      { get; }
        public float Y      { get; }
        public float Angle  { get; }
        public float Radius { get; }
    }
}
