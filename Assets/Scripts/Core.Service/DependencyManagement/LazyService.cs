namespace Core.Service.DependencyManagement
{
    public class LazyService<T>
    {
        private T service;

        public LazyService()
        {
        }

        public T Value
        {
            get
            {
                if (service == null)
                {
                    service = ObjectFactory.Instance.GetService<T>();
                }
                if (service == null)
                {
                    throw new System.Exception($"Service {typeof(T).Name} not found");
                }
                return service;
            }
        }
    }
}
