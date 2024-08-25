using Core.Game.World.EntityInterfaces;

namespace Core.Game.World.Service
{
    internal class EntityManager : IEntityManager
    {
        public void KillEntity(IKillableEntity killableEntity)
        {
            killableEntity.Kill();
        }

        public ISpawableEntity SpawnEntity(ISpawableEntity spawnableEntity)
        {
            var template = spawnableEntity.Configuration.Template;
            return spawnableEntity;
        }
    }
}
