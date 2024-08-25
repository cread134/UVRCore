using Core.Service.Application;
using Core.Service.AudioManagement;
using Core.Service.Logging;

namespace Core.Service.DependencyManagement
{
    internal class ServiceConfiguration : IServiceConfigurator
    {
        public void ConfigureServices(ObjectFactory objectFactory)
        {
            objectFactory.RegisterService<IAppSettings, AppSettings>();
            objectFactory.RegisterService<IAudioService, AudioService>();


            ConfigureLogging(objectFactory);
        }

        void ConfigureLogging(ObjectFactory objectFactory)
        {
            objectFactory.RegisterService<ILoggingService, LoggingService>();
        }
    }
}
