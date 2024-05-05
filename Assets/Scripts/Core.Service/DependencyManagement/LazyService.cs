namespace Core.Service.DependencyManagement
{
    public class LazyService<T> where T : IGameService
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
                    service = ObjectFactory.ResolveService<T>();
                }
                return service;
            }
        }
    }
}
