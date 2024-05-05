using UnityEngine;

namespace Core.Game
{
    [CreateAssetMenu]
    public class GunConfiguration : ScriptableObject
    {
        public Vector3 PositionalRecoil = new Vector3(0, 0, -0.1f);
        public Vector3 AngularRecoil = new Vector3(15, 0, 0);
    }
}
