using Core.Game.World.EntityInterfaces;

namespace Core.Game.World.Service
{
    public interface IEntityManager
    {
        void DamageEntity(IHealthEntity damageableEntity, int damage);
        void KillEntity(IKillableEntity killableEntity);
        ISpawableEntity SpawnEntity(ISpawableEntity spawnableEntity);
    }
}
