using UnityEngine.Profiling;

public static class LuaProfilerExport
{
    public static void BeginSample(int stringId)
    {
        var sampleName = LuaStringService.instance.GetString(stringId);
        Profiler.BeginSample(sampleName);
    }

    public static void EndSample()
    {
        Profiler.EndSample();
    }
}