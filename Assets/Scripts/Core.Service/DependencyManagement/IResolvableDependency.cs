using System;

namespace Core.Service.DependencyManagement
{
    internal class ResolvableDependency<T>
    {
        T _instance;
        internal T Resolve(ObjectFactory factory)
        {
            return _instance ??= CreateInstance(factory);
        }

        internal T CreateInstance(ObjectFactory factory)
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
    }
}