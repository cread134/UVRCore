namespace Core.Game.Interactables.Weapons.Firearms
{
    public interface IAmmunitionSource
    {
        public AmmoGroupConfiguration Configuration { get; }
        public bool Take();
        public bool Peek();
        public bool Put();
    }
}
