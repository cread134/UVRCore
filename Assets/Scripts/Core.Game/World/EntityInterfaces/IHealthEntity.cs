namespace Core.Game.World.EntityInterfaces
{
    public interface IHealthEntity : IWorldEntity
    {
        public int GetHealth();
        public int SetHealth(int health);
        public int Damage(int damage);
    }
}
