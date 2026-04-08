using System;
using System.IO;
using System.Text;
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
            }
            return _instance;
        }
    }
    private StreamWriter logWritter;

    public static void LogInfo(string format, params (string key, string value)[] logParams)
    {
        Logger.Instance._LogInfo(format, logParams);
    }

    public static void LogWarn(string format, params (string key, string value)[] logParams)
    {
        Logger.Instance._LogWarn(format, logParams);
    }

    public static void LogError(string logText, params (string key, string value)[] logParams)
    {
        Logger.Instance._LogError(logText, logParams);
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
            logWritter = null;
        }
    }

    private void OnUnityLogReceived(string logString, string stackTrace, LogType type)
    {
        if (logWritter == null || !GlobalConfig.ENABLE_LOG_WIRTER) {
            return;
        }

        DateTime curTime = DateTime.Now;
        if (type == LogType.Error) {
            logString = $"[{curTime:yyyy/MM/dd hh:mm:ss}][{type}] {logString} {stackTrace}";
        } else {
            logString = $"[{curTime:yyyy/MM/dd hh:mm:ss}][{type}] {logString}";
        }
        logWritter.WriteLine(logString);
        logWritter.Flush();
    }

    private void _LogInfo(string logText, params (string key, string value)[] logParams)
    {
        var sb = new StringBuilder();
        sb.AppendLine(logText);

        if (logParams != null && logParams.Length > 0) {
            foreach (var kvp in logParams) {
                sb.AppendLine($"#{kvp.key}: {kvp.value}");
            }
        }

        Debug.Log(sb.ToString());
    }

    private void _LogWarn(string logText, params (string key, string value)[] logParams)
    {
        var sb = new StringBuilder();
        sb.AppendLine(logText);

        if (logParams != null && logParams.Length > 0) {
            foreach (var kvp in logParams) {
                sb.AppendLine($"#{kvp.key}: {kvp.value}");
            }
        }

        Debug.LogWarning(sb.ToString());
    }

    private void _LogError(string logText, params (string key, string value)[] logParams)
    {
        var sb = new StringBuilder();
        sb.AppendLine(logText);

        if (logParams != null && logParams.Length > 0) {
            foreach (var kvp in logParams) {
                sb.AppendLine($"#{kvp.key}: {kvp.value}");
            }
        }
        Debug.LogError(sb.ToString());
    }
}