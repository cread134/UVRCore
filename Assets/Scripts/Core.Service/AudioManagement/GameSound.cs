using UnityEngine;

namespace Core.Service.AudioManagement
{
    [CreateAssetMenu]
    public class GameSound : ScriptableObject
    {
        public AudioClip AudioClips;

        public float Volume = 1;
        public float RandomPitch = 0f;
    }
}
