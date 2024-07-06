using UnityEngine;

namespace Core.Game.Interactables.Weapons.Firearms
{
    [CreateAssetMenu(fileName = "GunConfiguration", menuName = "Core/Game/Interactables/Weapons/Firearms/GunConfiguration")]
    public class GunConfiguration : ScriptableObject
    {
        public Vector3 PositionalRecoil = new Vector3(0, 0, -0.1f);
        public Vector3 AngularRecoil = new Vector3(15, 0, 0);
    }
}
