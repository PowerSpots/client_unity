using System.Diagnostics;

public class StopwatchExport
{
    public static bool IsHighResolution()
    {
        return Stopwatch.IsHighResolution;
    }

    public static long GetFrequency()
    {
        return Stopwatch.Frequency;
    }

    public static long GetTimestamp()
    {
        return Stopwatch.GetTimestamp();
    }
}
