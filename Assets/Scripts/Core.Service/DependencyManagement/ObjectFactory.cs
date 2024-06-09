using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Service.DependencyManagement
{
    public class ObjectFactory
    {
        public static ObjectFactory Instance => instance ??= InitializeDependencies();
        static ObjectFactory instance;

        Dictionary<Type, DependencyWrapper> services = new Dictionary<Type, DependencyWrapper>();
        List<Type> implementationTypes = new List<Type>();
        
       static ObjectFactory InitializeDependencies()
        {
            var factoryInstance = new ObjectFactory();

            var serviceConfigurators = GetConfigurators();
            foreach (var configurator in serviceConfigurators)
            {
                Debug.Log($"Configuring services with {configurator.GetType().Name}");
                configurator.ConfigureServices(factoryInstance);
            }

            return factoryInstance;
        }

        static List<IServiceConfigurator> GetConfigurators()
        {
            var type = typeof(IServiceConfigurator);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p) && p.IsClass)
                .ToList();
            return types.Select(t =>
            {
                try
                {
                    var instance = (IServiceConfigurator)Activator.CreateInstance(t);
                    return instance;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to create service instance of {t.Name}: {e.Message}");
                    return null;
                }
            }).ToList();
        }

        public void RegisterService<I, T>(DependencyType dependencyType = DependencyType.Singleton) where T : I
        {
            Console.WriteLine($"Registering service {typeof(T).Name} as {typeof(I).Name}");

            if (HasServiceInterface(typeof(I)))
            {
                Debug.LogError($"Service {typeof(I).Name} already registered");
                return;
            }

            if (IsServiceCyclical(typeof(T)))
            {
                Debug.LogError($"Cyclical dependency detected for {typeof(T).Name}");
                return;
            }

            var wrapper = new DependencyWrapper(dependencyType, typeof(T));
            services[typeof(I)] = wrapper;
            implementationTypes.Add(typeof(T));
        }

        bool IsServiceCyclical(Type implementation)
        {
            var dependencies = GetServiceDependencies(implementation);
            foreach (var dependency in dependencies)
            {
                if (ServiceHasDependency(dependency))
                {
                    return true;
                }
            }

            return false;
        }

        bool ServiceHasDependency(Type implementation)
        {
            return GetServiceDependencies(implementation).Contains(implementation);
        }

        List<Type> GetServiceDependencies(Type implementation)
        {
            if (implementation.IsSubclassOf(typeof(MonoBehaviour)))
            {
                return GetMonoBehaviourDependencies(implementation);
            }
            else
            {
                return GetClassDependencies(implementation);
            }
        }

        List<Type> GetClassDependencies(Type implementation)
        {
            var constructor = implementation.GetConstructors().FirstOrDefault();
            if (constructor == null)
            {
                return new List<Type>();
            }
            return constructor.GetParameters().Select(p => p.ParameterType).ToList();
        }

        List<Type> GetMonoBehaviourDependencies(Type implementation)
        {
            var injectMethods = implementation.GetMethods().Where(m => m.GetCustomAttributes(typeof(InjectAttribute), false).Length > 0);
            if (injectMethods.Count() == 0)
            {
                return new List<Type>();
            }
            return injectMethods.SelectMany(m => m.GetParameters().Select(p => p.ParameterType))
                                .Distinct()
                                .ToList();
        }

        public T GetService<T>()
        {
            return (T)GetService(typeof(T));
        }
        internal object GetService(Type type)
        {
            if (!services.ContainsKey(type))
            {
                Debug.LogError($"Service {type.Name} not registered");
                return null;
            }
            return services[type].Retrieve();
        }

        internal bool HasServiceInterface(Type parameterType)
        {
            return services.ContainsKey(parameterType);
        }
        internal bool HasServiceImplementation(Type parameterType)
        {
            return implementationTypes.Contains(parameterType);
        }
    }
}
