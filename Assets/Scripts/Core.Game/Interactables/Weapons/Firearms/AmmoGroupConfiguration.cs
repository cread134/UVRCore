using UnityEngine;

namespace Core.Game.Interactables.Weapons.Firearms
{
    [CreateAssetMenu(fileName = "AmmoGroupConfiguration", menuName = "Core/Game/Interactables/Weapons/Firearms/AmmoGroupConfiguration")]
    public class AmmoGroupConfiguration : ScriptableObject
    {
        public AmmunitionKey AmmunitionKey = AmmunitionKey.None;
        public int MaxQuantity = 30;
    }
}
