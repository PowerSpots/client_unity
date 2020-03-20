using Gankx;
using XLua;

public class PerformanceCounterExport
{
    public static void BeginAccumulate(string name)
    {
        PerformanceCounterService.instance.Begin(name);
    }

    public static void EndAccumulate(string name)
    {
        PerformanceCounterService.instance.End(name);
    }

    public static void BeginRecord()
    {
        PerformanceCounterService.instance.StartRecord();
    }

    public static void EndRecord()
    {
        PerformanceCounterService.instance.StopRecord();
    }

    public static LuaTable GetTimeDict()
    {
        return GetLuaMap(PerformanceCounterService.instance.GetSortedRoot());
    }

    public static float GetRecordedDuration()
    {
        return PerformanceCounterService.instance.GetRecordDuration();
    }

    public static LuaTable GetLuaMap(PerformanceCounterService.CounterTask task)
    {
        var luaTable = LuaService.instance.NewTable();
        luaTable.Set("name", task.name);
        luaTable.Set("time", task.GetElapsedMilliseconds());
        using (var childrenTable = LuaService.instance.NewTable())
        {
            for (var i = 0; i < task.childCounters.Count; i++)
            {
                var child = GetLuaMap(task.childCounters[i]);
                child.Set("parent", luaTable);
                childrenTable.Set(i + 1, child);
                child.Dispose();
            }

            luaTable.Set("children", childrenTable);
        }

        return luaTable;
    }
}