using Core.Service.Application;
using Core.Service.AudioManagement;
using Core.Service.Logging;

namespace Core.Service.DependencyManagement
{
    internal class ServiceConfiguration
    {
        internal static void RegisterMonoBehaviours(ObjectFactory objectFactory)
        {
            objectFactory.RegisterCore<IAppSettings>(AppSettings.Instance);
            objectFactory.RegisterCore<IAudioService>(AudioService.Instance);
        }

        internal static void RegisterServices()
        {
        }

        internal static ILoggingService ConfigureLogging(ObjectFactory objectFactory)
        {
            var instance = new LoggingService();
            objectFactory.RegisterCore<ILoggingService>(instance);
            return instance;
        }
    }
}
