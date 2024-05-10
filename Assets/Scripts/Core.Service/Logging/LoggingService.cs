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
        internal LoggingService()
        {
            UnityEngine.Application.logMessageReceived += ApplicationLogMessageReceived;
        }

        private void ApplicationLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                Log(condition, LogLevel.Error);
                Log(stackTrace, LogLevel.Error);
            }
        }

        public void Log(StructuredLog log)
        {
            var serializedLog = log.Serialize();
            if (AppSettings.IsEditor)
            {
                var context = log.TryGetPropertyValue("ObjectContext", out var objectContext) ? objectContext as UnityEngine.Object : null;
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

        public void Log(string message, LogLevel logType = LogLevel.Info, UnityEngine.Object context = null)
        {
            LogBuilder.CreateLog(message).WithLogLevel(logType).WithContext(context).Post();
        }
    }
}
