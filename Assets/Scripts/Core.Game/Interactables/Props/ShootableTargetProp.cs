using Core.Game.World;
using Core.Game.World.EntityInterfaces;
using UnityEngine;

namespace Core.Game.Interactables.Props
{
    internal class ShootableTargetProp : MonoBehaviour, IWorldEntity, IHealthEntity, IKillableEntity
    {
        [SerializeField] private EntityConfiguration _configuration;
        public EntityConfiguration Configuration => _configuration;

        [EntityProperty, SerializeField]
        private int _health = 100;

        public int Damage(int damage)
        {
            _health -= damage;
            if (_health <= 0)
            {
                Kill();
            }
            return _health;
        }

        public int GetHealth() => _health;

        public int SetHealth(int health)
        {
            _health = health;
            return _health;
        }

        public void Kill()
        {
        }
    }
}
