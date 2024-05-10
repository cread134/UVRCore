using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Core.Service.Logging
{
    public class StructuredLog
    {
        public static StructuredLog Default
        {
            get
            {
                var instance = new StructuredLog();
                instance.AddProperty("Message", "");
                instance.AddProperty("LogLevel", LogLevel.Info);
                instance.AddProperty("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                instance.AddProperty("GameTime", Time.time);
                instance.AddProperty("ObjectContext",  "");
                instance.AddProperty("Thread", System.Threading.Thread.CurrentThread.ManagedThreadId);
                instance.AddProperty("UnityVersion", UnityEngine.Application.version);
                return instance;
            }
        }

        Dictionary<string, object> _properties = new Dictionary<string, object>();
        public void AddProperty(string key, object value)
        {
            _properties.Add(key, value);
        }

        public bool TryGetPropertyValue(string key, out object valueResolver)
        {
            return _properties.TryGetValue(key, out valueResolver);
        }

        public string Serialize()
        {
            var sb = new StringBuilder();
            foreach (var item in _properties)
            {
                var value = item.Value;
                if (value is null)
                {
                    continue;
                }
                sb.AppendLine($"{item.Key}: {item.Value}");
            }
            return sb.ToString();
        }

        internal void SetProperty(string key, object value)
        {
            if (_properties.ContainsKey(key))
            {
                _properties[key] = value;
            } 
            else
            {
                AddProperty(key, value);
            }
        }
    }
}
