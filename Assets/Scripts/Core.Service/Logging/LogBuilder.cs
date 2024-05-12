using Core.Service.DependencyManagement;
using System;

namespace Core.Service.Logging
{
    public class LogBuilder
    {
        public LogBuilder(StructuredLog log)
        {
            _log = log;
        }
        StructuredLog _log;
        LogLevel logType = LogLevel.Info;
        UnityEngine.Object context = null;

        public LogBuilder WithContext(UnityEngine.Object context)
        {
            this.context = context;
            return this;
        }

        public LogBuilder WithLogLevel(LogLevel logType)
        {
            this.logType = logType;
            return this;
        }

        public LogBuilder Set(string key, string value)
        {
            if (_log.HasProperty(key))
            {
                _log.SetProperty(key, value);
            }
            else
            {
                _log.AddProperty(key, value);
            }
            return this;
        }

        public StructuredLog Build()
        {
            _log.SetProperty("ObjectContext", context);
            _log.SetProperty("LogLevel", logType);
            return _log;
        }

        public static LogBuilder CreateLog(string message)
        {
            var instance = StructuredLog.Default;
            instance.SetProperty("Message", message);
            return new LogBuilder(instance);
        }
    }
}
