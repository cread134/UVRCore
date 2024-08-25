using Core.Game.World.EntityInterfaces;

namespace Core.Game.World.Service
{
    public interface IEntityManager
    {
        void KillEntity(IKillableEntity killableEntity);
        ISpawableEntity SpawnEntity(ISpawableEntity spawnableEntity);
    }
}
