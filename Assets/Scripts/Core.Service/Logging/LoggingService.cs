using Core.Service.Application;
using System;
using UnityEngine;

namespace Core.Service.Logging
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Exception
    }
    internal class LoggingService : ILoggingService
    {
        public EventHandler<StructuredLog> OnLog { get; set; }

        internal LoggingService()
        {
            UnityEngine.Application.logMessageReceived += ApplicationLogMessageReceived;
        }

        private void ApplicationLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                var message = $"Exception: {condition}";
                LogBuilder.CreateLog(message)
                          .WithLogLevel(LogLevel.Exception)
                          .Set("StackTrace", stackTrace)
                          .Post();
            }
        }

        public void Log(string message, LogLevel logType = LogLevel.Info, UnityEngine.Object context = null)
        {
            LogBuilder.CreateLog(message).WithLogLevel(logType).WithContext(context).Post();
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

            OnLog?.Invoke(this, log);
        }
    }
}
