using UnityEngine;

namespace Core.Service.GameEffects
{
    [CreateAssetMenu(fileName = "GameEffect", menuName = "Game/Game Effect")]
    public class GameEffect : ScriptableObject
    {
        public string Key => EffectPrefab.name;
        public GameObject EffectPrefab;
    }
}
