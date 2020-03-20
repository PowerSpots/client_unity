using System.IO;
using System.Text;
using Gankx;
using UnityEngine;
using UnityEngine.Profiling;

public class DebugExport
{
    public enum LogLevel
    {
        Error = 0,
        Warning,
        Info
    }

    public static LogLevel logLevel = LogLevel.Warning;

    public static void WriteFile(string path, string content)
    {
        path = Directory.GetCurrentDirectory() + "/" + path;

        var directory = Path.GetDirectoryName(path);

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using (var file = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
        {
            using (var sw = new StreamWriter(file, Encoding.Default))
            {
                sw.Write(content);
            }
        }
    }

    public static void RegisterLog()
    {
        Application.logMessageReceived += HandleLog;
    }

    public static void CancelLog()
    {
        Application.logMessageReceived -= HandleLog;
    }

    public static void LogError(string message)
    {
        Profiler.BeginSample("DebugExport.LogError");
        Debug.LogError(message);
        Profiler.EndSample();
    }

    public static void LogWarning(string message)
    {
        Profiler.BeginSample("DebugExport.LogError");
        Debug.LogWarning(message);
        Profiler.EndSample();
    }

    public static void Log(string message)
    {
        Profiler.BeginSample("DebugExport.LogError");
        Debug.Log(message);
        Profiler.EndSample();
    }

    public static void LogNetwork(string message)
    {
        Profiler.BeginSample("DebugExport.LogNetwork");
        // TODO DebugFileLog
//        DebugFileLog.Instance.LogNetwork(message);
        Profiler.EndSample();
    }

    public static void ClearLog()
    {
        LuaService.instance.FireEvent("ClearLog");
    }

    private static void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Warning)
        {
            if (logLevel >= LogLevel.Warning)
            {
                LuaService.instance.FireEvent("DoLogWarning", logString);
            }
        }
        else if (type == LogType.Error)
        {
            if (logLevel >= LogLevel.Error)
            {
                LuaService.instance.FireEvent("DoLogError", logString);
            }
        }
        else if (type == LogType.Exception)
        {
            if (logLevel >= LogLevel.Error)
            {
                LuaService.instance.FireEvent("DoLogError", logString + stackTrace);
            }
        }
        else
        {
            if (logLevel >= LogLevel.Info)
            {
                LuaService.instance.FireEvent("DoLog", logString);
            }
        }
    }
}