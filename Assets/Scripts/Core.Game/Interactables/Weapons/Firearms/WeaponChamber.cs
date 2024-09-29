namespace Core.Game.Interactables.Weapons.Firearms
{
    public class WeaponChamber : IAmmunitionSource
    {
        public AmmoGroupConfiguration Configuration { get; } = null;

        public bool Peek()
        {
            return HasAmmunition;
        }

        public bool Take()
        {
            if (HasAmmunition)
            {
                HasAmmunition = false;
                return true;
            }
            return false;
        }

        public bool Put()
        {
            if (!HasAmmunition)
            {
                HasAmmunition = true;
                return true;
            }
            return false;
        }

        public bool HasAmmunition { get; set; } = false;
    }
}
