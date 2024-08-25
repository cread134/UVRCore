namespace Core.Service.DependencyManagement
{
    public interface IServiceConfigurator
    {
        public void ConfigureServices(ObjectFactory objectFactory);
    }
}
