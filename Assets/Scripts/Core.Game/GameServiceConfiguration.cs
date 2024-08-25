using Core.Game.World.Service;
using Core.Service.DependencyManagement;
using Core.XRFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Game.Assets.Scripts.Core.Game
{
    public class GameServiceConfiguration : IServiceConfigurator
    {
        public void ConfigureServices(ObjectFactory objectFactory)
        {
            objectFactory.RegisterService<IEntityManager, EntityManager>();
        }
    }
}
