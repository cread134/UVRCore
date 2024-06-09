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

        public LoggingService()
        {
            UnityEngine.Application.logMessageReceived += ApplicationLogMessageReceived;
        }

        private void ApplicationLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception)
            {
                try 
                {
                    var message = $"Exception: {condition}";
                    LogBuilder.CreateLog(message)
                              .WithLogLevel(LogLevel.Exception)
                              .Set("StackTrace", stackTrace)
                              .Build();
                } catch
                {
                }
            }
        }

        public void Log(string message, LogLevel logType = LogLevel.Info, UnityEngine.Object context = null)
        {
           var log = LogBuilder.CreateLog(message).WithLogLevel(logType).WithContext(context).Build();
           PostLog(log);
        }

        public void PostLog(StructuredLog log)
        {
            var serializedLog = log.Serialize();
            if (UnityEngine.Application.isEditor)
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
