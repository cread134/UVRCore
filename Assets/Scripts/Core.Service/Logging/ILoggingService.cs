using UnityEngine;

namespace Core.Service.Logging
{
    public interface ILoggingService
    {
        void Log(string message, LogLevel logType = LogLevel.Info, Object context = null);
    }
}