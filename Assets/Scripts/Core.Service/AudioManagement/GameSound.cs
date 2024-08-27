using UnityEngine;

namespace Core.Service.AudioManagement
{
    [CreateAssetMenu(fileName = "GameSound", menuName = "Core/Game/AV/Game Sound")]
    public class GameSound : ScriptableObject
    {
        public string SoundName;
        public AudioClip[] AudioClips;

        public float VolumeMultiplier = 1;
        public float RandomPitch = 0f;
        public float MinRangeMultiplier = 1f;
        public float MaxRangeMultiplier = 1f;
    }
}
