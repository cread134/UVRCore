using Core.Service.Logging;
using System;
using System.Collections.Generic;

namespace Core.Service.DependencyManagement
{
    public class ObjectFactory
    {
        static ObjectFactory _instance;
        public static ObjectFactory Instance => _instance ??= new ObjectFactory();

        Dictionary<Type, Func<object>> objectRegistry;
        ILoggingService loggingService;

        ObjectFactory()
        {
            objectRegistry = new Dictionary<Type, Func<object>>();
            loggingService = ServiceConfiguration.ConfigureLogging(this);
            ServiceConfiguration.RegisterServices();
            ServiceConfiguration.RegisterMonoBehaviours(this);
        }

        public static void RegisterService<T>(object instance)
        {
            if (!UnityEngine.Application.isPlaying)
            {
                return;
            }
            Instance.RegisterCore<T>(instance);
        }

        internal void RegisterCore<T>(object instance)
        {
            loggingService?.Log($"Registering service {typeof(T).Name}");
            objectRegistry.TryAdd(typeof(T), () => instance);
        }

        public static T ResolveService<T>()
        {
            if (!UnityEngine.Application.isPlaying)
            {
                return default(T);
            }
            return Instance.ResolveCore<T>();
        }

        internal T ResolveCore<T>()
        {
            if (objectRegistry.TryGetValue(typeof(T), out var resolver))
            {
                var instance = resolver();
                if (instance is T typeConverted)
                {
                    return typeConverted;
                }
            }
            return default(T);
        }

        internal void RegisterDependency<T>() where T : class
        {
            var resolver = new ResolvableDependency<T>();
            objectRegistry.TryAdd(typeof(T), () => resolver.Resolve(this));
        }
    }
}
