using UnityEngine;

namespace Core.Game.Interactables.Weapons.Firearms
{
    [CreateAssetMenu(fileName = "GunConfiguration", menuName = "Core/Game/Interactables/Weapons/Firearms/GunConfiguration")]
    public class GunConfiguration : ScriptableObject
    {
        [Header("Recoil Settings")]
        public Vector3 PositionalRecoil = new Vector3(0, 0, -0.1f);
        public Vector3 AngularRecoil = new Vector3(15, 0, 0);

        [Header("Ammunition Settings")]
        public AmmunitionKey AmmunitionKey;
        public AmmutionStorageType AmmunitionStorageType;
    }

    public enum AmmutionStorageType
    {
        Magazine = 0,
        Integrated = 1
    }
}
