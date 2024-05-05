using System;
using UnityEngine;

namespace Core.Service.AudioManagement
{
    public class AudioInstance : MonoBehaviour
    {
        public static AudioInstance Create(Transform parent)
        {
            var instance = new GameObject($"AudioInstance_{Guid.NewGuid()}");
            var audioSource = instance.AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1;

            var audioInstance = instance.AddComponent<AudioInstance>();
            instance.transform.SetParent(parent);
            instance.transform.position = parent.position;

            return audioInstance;
        }

        AudioSource _audioSource;
        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
        }
    }
}
