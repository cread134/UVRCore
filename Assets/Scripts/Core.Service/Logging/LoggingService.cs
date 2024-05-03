using Core.Service.Application;
using System;
using System.Collections;
using UnityEngine;

namespace Core.Service.Logging
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Fatal
    }
    internal class LoggingService : ILoggingService
    {
        private StructuredLog _structuredLog;

        internal LoggingService()
        {
            this._structuredLog = new StructuredLog();
            _structuredLog.AddProperty("Message", () => "");
            _structuredLog.AddProperty("LogLevel", () => LogLevel.Info.ToString());
            _structuredLog.AddProperty("Timestamp", () => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            _structuredLog.AddProperty("GameTime", () => Time.time.ToString());
            _structuredLog.AddProperty("ObjectContext", () => "");
        }

        public void Log(string message, LogLevel logType = LogLevel.Info, UnityEngine.Object context = null)
        {
            _structuredLog.SetProperty("Message", message);
            _structuredLog.SetProperty("LogLevel", logType.ToString());
            _structuredLog.SetProperty("ObjectContext", context?.name ?? "");
            var serializedLog = _structuredLog.Serialize();
            if (AppSettings.IsEditor)
            {
                if (context == null)
                {
                    Debug.Log(serializedLog);
                }
                else
                {
                    Debug.Log(serializedLog, context);
                }
            }
        }
    }
}
