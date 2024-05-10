using Core.Service.DependencyManagement;
using UnityEngine;

namespace Core.Service.Logging
{
    public interface ILoggingService : IGameService
    {
        void Log(string message, LogLevel logType = LogLevel.Info, Object context = null);
        void Log(StructuredLog log);
    }
}