using System;
using System.IO;
using UnityEngine;

public class Logger
{
    private static Logger _instance;
    public static Logger Instance
    {
        get
        {
            if (_instance == null) {
                _instance = new Logger();
                _instance.Init();
            }
            return _instance;
        }
    }
    private StreamWriter logWritter;

    public static void LogInfo(string format, params object[] args)
    {
        Logger.Instance._LogInfo(format, args);
    }

    public static void LogWarn(string format, params object[] args)
    {
        Logger.Instance._LogWarn(format, args);
    }

    public static void LogError(string format, params object[] args)
    {
        Logger.Instance._LogError(format, args);
    }

    public void Init()
    {
        Application.logMessageReceived += OnUnityLogReceived;
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

        try {
            DateTime curDate = DateTime.Now;
            string dayStr = $"log-{curDate:yyyy-MM-dd}";
            string logDayDir = Path.Combine(GlobalConfig.PATH_LOG, dayStr);
            if (!Directory.Exists(logDayDir)) {
                Directory.CreateDirectory(logDayDir);
            }
            string logFilePath = Path.Combine(logDayDir, $"{dayStr}.txt");
            logWritter = new StreamWriter(logFilePath, false, System.Text.Encoding.UTF8);
        }
        catch (Exception e) {
            Debug.LogError("Create log file writter failed: " + e);
        }
    }

    public void Destroy()
    {
        if (logWritter != null) {
            logWritter.Flush();
            logWritter.Close();
        }
    }

    private void OnUnityLogReceived(string logString, string stackTrace, LogType type)
    {
        if (logWritter == null || !GlobalConfig.ENABLE_LOG_WIRTTER) {
            return;
        }

        DateTime curTime = DateTime.Now;
        logString = $"[{curTime:yyyy/MM/dd hh:mm:ss}][{type}] {logString} {stackTrace}";
        logWritter.WriteLine(logString);
        logWritter.Flush();
    }

    private void _LogInfo(string format, params object[] args)
    {
        Debug.LogFormat(format, args);
    }

    private void _LogWarn(string format, params object[] args)
    {
        Debug.LogWarningFormat(format, args);
    }

    private void _LogError(string format, params object[] args)
    {
        Debug.LogErrorFormat(format, args);
    }
}