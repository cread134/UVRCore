using System;
using System.Collections;
using UnityEngine;

namespace Core.Service.AudioManagement
{
    public class AudioInstance : MonoBehaviour
    {
        public static AudioInstance Create(Transform parent, Action<AudioInstance> finishedCallback)
        {
            var instance = new GameObject($"AudioInstance_{Guid.NewGuid()}");
            var audioSource = instance.AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1;

            var audioInstance = instance.AddComponent<AudioInstance>();
            instance.transform.SetParent(parent);
            instance.transform.position = parent.position;
            instance.hideFlags = HideFlags.HideInHierarchy;
            audioInstance.finishedCallback = finishedCallback;

            return audioInstance;
        }

        public bool IsPlaying { get; private set; } = false;
        Coroutine playCoroutine;
        public void PlaySound(GameSound gameSound, bool isLoop, AudioOverride? audioOverride = null)
        {
            var targetClip = gameSound.AudioClips[UnityEngine.Random.Range(0, gameSound.AudioClips.Length)];

            audioOverride ??= AudioOverride.Default;

            _audioSource.clip = targetClip;
            _audioSource.volume = baseSoundVolume * gameSound.VolumeMultiplier * audioOverride.Value.VolumeMultiplier;
            _audioSource.pitch = (basePitch + UnityEngine.Random.Range(-gameSound.RandomPitch, gameSound.RandomPitch)) * audioOverride.Value.PitchMultiplier;
            _audioSource.maxDistance = baseMaxDistance * gameSound.MaxRangeMultiplier;
            _audioSource.minDistance = baseMinDistance * gameSound.MinRangeMultiplier;

            IsPlaying = true;
            if (isLoop)
            {
                _audioSource.loop = true;
            }
            else
            {
                _audioSource.loop = false;
                if (playCoroutine != null)
                {
                    StopCoroutine(playCoroutine);
                }
                playCoroutine = StartCoroutine(PlaySoundCoroutine(targetClip.length));
            }
            _audioSource.Play();
        }

        private IEnumerator PlaySoundCoroutine(float length)
        {
            yield return new WaitForSeconds(length);
            StopPlaying();
        }

        public void StopPlaying()
        {
            _audioSource.Stop();
            IsPlaying = false;
            finishedCallback?.Invoke(this);
        }

        AudioSource _audioSource;
        Action<AudioInstance> finishedCallback;
        float baseSoundVolume = 1;
        float basePitch = 1;
        float baseMaxDistance = 1;
        float baseMinDistance = 1;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            baseSoundVolume = _audioSource.volume;
            basePitch = _audioSource.pitch;
            baseMaxDistance = _audioSource.maxDistance;
            baseMinDistance = _audioSource.minDistance;
        }
    }
}
