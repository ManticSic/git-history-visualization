namespace gitHistoryVisualization;


public struct ArchimedeanSpiral
{
    public ArchimedeanSpiral(float size, int stepsPerRevolution, DateTime startDate, DateTime endDate)
    {
        OuterDiameter      = size / 2;
        InnerDiameter      = size / 6;
        Increment          = (float)(2 * Math.PI / stepsPerRevolution);
        StartAngle         = (float)((startDate.DayOfYear / 365f * 360 + 270) * Math.PI / 180);
        Turns              = endDate.Year - startDate.Year + 1;
    }

    public float OuterDiameter      { get; }
    public float InnerDiameter      { get; }
    public float Increment          { get; }
    public float StartAngle         { get; }
    public int   Turns              { get; }
}
