using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Service.Logging
{
    internal class StructuredLog
    {
        Dictionary<string, Func<string>> _properties = new Dictionary<string, Func<string>>();
        public void AddProperty(string key, Func<string> valueResolver)
        {
            _properties.Add(key, valueResolver);
        }

        public void GetPropertyValue(string key, out Func<string> valueResolver)
        {
            _properties.TryGetValue(key, out valueResolver);
        }

        public string Serialize()
        {
            var sb = new StringBuilder();
            foreach (var item in _properties)
            {
                var value = item.Value();
                if (string.IsNullOrEmpty(value))
                {
                    continue;
                }
                sb.AppendLine($"{item.Key}: {item.Value()}");
            }
            return sb.ToString();
        }

        internal void SetProperty(string key, string value)
        {
            if (_properties.ContainsKey(key))
            {
                _properties[key] = () => value;
            } else
            {
                AddProperty(key, () => value);
            }
        }
    }
}
