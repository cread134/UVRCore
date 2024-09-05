using Core.Game.World.EntityInterfaces;
using Unity.Netcode;

namespace Core.Game.World.Service
{
    public class EntityManager : NetworkBehaviour, IEntityManager
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

        public void DamageEntity(IHealthEntity damageableEntity, int damage)
        {
            if (damageableEntity is NetworkBehaviour networkBehaviour)
            {
            }
        }
    }
}
