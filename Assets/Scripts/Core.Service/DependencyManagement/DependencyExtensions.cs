using System;
using System.Linq;
using System.Reflection;

namespace Core.Service.DependencyManagement
{
    public static class DependencyExtensions
    {
        public static void InjectServices(this object target)
        {
            var targetType = target.GetType();
            InjectFields(targetType, target);
            InjectProperties(targetType, target);
        }

        private static void InjectFields(Type targetType, object targetObject)
        {
            var injectableFields = targetType
                .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f.GetCustomAttributes(typeof(InjectAttribute), false).Any());

            foreach (var field in injectableFields)
            {
                var service = ObjectFactory.Instance.GetService(field.FieldType);
                if (service == null)
                {
                    throw new Exception($"Service {field.FieldType.Name} not found");
                }
                field.SetValue(targetObject, service);
            }
        }

        static void InjectProperties(Type targetType, object targetObject)
        {
            var injectableProperties = targetType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.GetCustomAttributes(typeof(InjectAttribute), false).Any());

            foreach (var property in injectableProperties)
            {
                var service = ObjectFactory.Instance.GetService(property.PropertyType);
                if (service == null)
                {
                    throw new Exception($"Service {property.PropertyType.Name} not found");
                }
                property.SetValue(targetObject, service);
            }
        }
    }
}
