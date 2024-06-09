using Core.Service.DependencyManagement;

namespace Core.XRFramework
{
    public class XrCoreServiceConfiguration : IServiceConfigurator
    {
        public void ConfigureServices(ObjectFactory objectFactory)
        {
            objectFactory.RegisterService<IHapticsService, HapticsService>();
        }
    }
}