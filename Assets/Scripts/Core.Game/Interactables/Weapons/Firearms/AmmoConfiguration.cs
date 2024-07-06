using UnityEngine;

namespace Core.Game.Interactables.Weapons.Firearms
{
    [CreateAssetMenu(fileName = "AmmoConfiguration", menuName = "Core/Game/Interactables/Weapons/Firearms/AmmoConfiguration")]

    public class AmmoConfiguration : ScriptableObject
    {
        public AmmunitionKey AmmunitionKey = AmmunitionKey.None;
        public int Damage = 10;
    }
}
