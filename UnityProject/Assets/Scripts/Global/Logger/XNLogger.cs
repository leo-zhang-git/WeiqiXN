using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace XNClient.Logger
{
    public class XNLogger
    {
        private static XNLogger _instance;
        public static XNLogger Instance
        {
            get
            {
                if (_instance == null) {
                    _instance = new XNLogger();
                }
                return _instance;
            }
        }
        private StreamWriter logWritter;

        public static void LogInfo(string format, params (string key, string value)[] logParams)
        {
            XNLogger.Instance._LogInfo(format, logParams);
        }

        public static void LogWarn(string format, params (string key, string value)[] logParams)
        {
            XNLogger.Instance._LogWarn(format, logParams);
        }

        public static void LogError(string logText, params (string key, string value)[] logParams)
        {
            XNLogger.Instance._LogError(logText, logParams);
        }

        public void Init()
        {
            Application.logMessageReceived += OnUnityLogReceived;
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

            try {
                DateTime curDate = DateTime.Now;
                string dayStr = $"log-{curDate:yyyy-MM-dd}";
                string logDayDir = Path.Combine(LoggerConfig.PATH_LOG, dayStr);
                if (!Directory.Exists(logDayDir)) {
                    Directory.CreateDirectory(logDayDir);
                }
                string logFilePath = Path.Combine(logDayDir, $"{curDate:yyyy-MM-dd-HHmmss}.txt");
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
            _instance = null;
        }

        private void OnUnityLogReceived(string logString, string stackTrace, LogType type)
        {
            if (logWritter == null || !LoggerConfig.ENABLE_LOG_WIRTER) {
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
            sb.Append(logText);

            if (logParams != null && logParams.Length > 0) {
                sb.AppendLine();
                foreach (var kvp in logParams) {
                    sb.Append($" #{kvp.key}: {kvp.value}");
                }
            }

            Debug.Log(sb.ToString());
        }

        private void _LogWarn(string logText, params (string key, string value)[] logParams)
        {
            var sb = new StringBuilder();
            sb.Append(logText);

            if (logParams != null && logParams.Length > 0) {
                sb.AppendLine();
                foreach (var kvp in logParams) {
                    sb.Append($" #{kvp.key}: {kvp.value}");
                }
            }

            Debug.LogWarning(sb.ToString());
        }

        private void _LogError(string logText, params (string key, string value)[] logParams)
        {
            var sb = new StringBuilder();
            sb.Append(logText);

            if (logParams != null && logParams.Length > 0) {
                sb.AppendLine();
                foreach (var kvp in logParams) {
                    sb.Append($" #{kvp.key}: {kvp.value}");
                }
            }
            Debug.LogError(sb.ToString());
        }
    }
}

