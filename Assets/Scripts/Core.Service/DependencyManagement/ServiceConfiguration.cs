using Core.Service.Application;
using Core.Service.Logging;

namespace Core.Service.DependencyManagement
{
    internal class ServiceConfiguration
    {
        internal static void RegisterServices(ObjectFactory objectFactory)
        {
            objectFactory.RegisterCore<IAppSettings>(AppSettings.Instance);
        }

        internal static ILoggingService ConfigureLogging(ObjectFactory objectFactory)
        {
            var instance = new LoggingService();
            objectFactory.RegisterCore<ILoggingService>(instance);
            return instance;
        }
    }
}
