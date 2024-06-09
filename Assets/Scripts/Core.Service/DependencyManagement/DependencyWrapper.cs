using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Service.DependencyManagement
{
    internal class DependencyWrapper
    {
        DependencyType dependencyType;
        Type dependencyObjectType;
        object instance;

        public Type DependencyObjectType { get => dependencyObjectType; set => dependencyObjectType = value; }
        public DependencyType DependencyType { get => dependencyType; set => dependencyType = value; }

        internal DependencyWrapper(DependencyType dependencyType, Type dependencyObjectType)
        {
            this.dependencyType = dependencyType;
            DependencyObjectType = dependencyObjectType;
        }

        public object Retrieve()
        {
            if (instance == null || dependencyType == DependencyType.Transient)
            {
                instance = CreateInstance(DependencyObjectType);
            }
            return instance;
        }

        object CreateInstance(Type type)
        {
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
            {
                return CreateMonoBehaviourInstance(type);
            }
            else
            {
                return CreateClassInstance(type);
            }
        }

        private static object CreateClassInstance(Type type)
        {
            Debug.Log($"Creating class instance of {type.Name}");
            var constructor = type.GetConstructors().FirstOrDefault();
            if (constructor == null)
            {
                Debug.LogError($"Failed to create instance of {type.Name} no constructor found");
                return null;
            }
            if (constructor.GetParameters().Length == 0)
            {
                return Activator.CreateInstance(type);
            }

            var parameters = constructor.GetParameters();
            var requiredTypeInstances = new List<object>();
            foreach (var parameter in parameters)
            {
                if (!ObjectFactory.Instance.HasServiceInterface(parameter.ParameterType))
                {
                    Debug.LogError($"Failed to resolve dependency for {parameter.ParameterType.Name} no service found");
                    return null;
                }
                var instance = ObjectFactory.Instance.GetService(parameter.ParameterType);
                requiredTypeInstances.Add(instance);
            }

            if (requiredTypeInstances.Count != parameters.Length)
            {
                Debug.LogError($"Failed to resolve all dependencies for {type.Name}");
            }
            else
            {
                return constructor.Invoke(requiredTypeInstances.ToArray());
            }
            return null;
        }

        private static object CreateMonoBehaviourInstance(Type type)
        {
            Debug.Log($"Creating monobehaviour instance of {type.Name}");
            var monoBehaviourInstanceName = type.Name + "Instance";
            var createdInstance = new GameObject(monoBehaviourInstanceName).AddComponent(type);

            var injectAttributes = type.GetMethods().Where(m => m.GetCustomAttributes(typeof(InjectAttribute), false).Length > 0);
            foreach (var injectAttribute in injectAttributes)
            {
                var parameters = injectAttribute.GetParameters();
                var requiredTypeInstances = new List<object>();
                foreach (var parameter in parameters)
                {
                    if (!ObjectFactory.Instance.HasServiceInterface(parameter.ParameterType))
                    {
                        Debug.LogError($"Failed to resolve dependency for {parameter.ParameterType.Name} inject method has non service parameters");
                        return null;
                    }
                    var instance = ObjectFactory.Instance.GetService(parameter.ParameterType);
                    requiredTypeInstances.Add(instance);
                }

                if (requiredTypeInstances.Count != parameters.Length)
                {
                    Debug.LogError($"Failed to resolve all dependencies for {injectAttribute.Name}");
                }
                else
                {
                    injectAttribute.Invoke(createdInstance, requiredTypeInstances.ToArray());
                }
            }

            MonoBehaviour.DontDestroyOnLoad((MonoBehaviour)(createdInstance));
            return createdInstance;
        }
    }

}
