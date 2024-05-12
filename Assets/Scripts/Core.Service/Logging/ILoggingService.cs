using Core.Service.DependencyManagement;
using System;
using UnityEngine;

namespace Core.Service.Logging
{
    public interface ILoggingService : IGameService
    {
        EventHandler<StructuredLog> OnLog { get; set; }
        void Log(string message, LogLevel logType = LogLevel.Info, UnityEngine.Object context = null);
        void PostLog(StructuredLog log);
    }
}