using UnityEngine;

namespace Core.Service.GameEffects
{
    [CreateAssetMenu(fileName = "GameEffect", menuName = "Core/Game/AV/Game Effect")]
    public class GameEffect : ScriptableObject
    {
        public string Key => EffectPrefab.name;

        public GameObject EffectPrefab;
    }
}
