using Core.Service.DependencyManagement;
using System.Collections;
using System.Collections.Generic;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEngine;

namespace Core.Service.AudioManagement
{
    internal class AudioService : SingletonClass<AudioService>, IAudioService
    {
        protected override void OnCreated()
        {
            CreatePool();
        }

        const int _poolSize = 40;
        Queue<AudioInstance> _pool = new Queue<AudioInstance>();
        Queue<AudioInstance> _playingPool = new Queue<AudioInstance>();

        bool _isInitialized;
        void CreatePool()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                var instance = AudioInstance.Create(transform, OnSoundFinish);
                _pool.Enqueue(instance);
            }
            _isInitialized = true;
        }

        void OnSoundFinish(AudioInstance instance)
        {
            if (_playingPool.Contains(instance))
            {
                _playingPool.Dequeue();
                _pool.Enqueue(instance);
            }
        }

        public void PlaySound(GameSound gameSound, Vector3? position = null, Quaternion? rotation = null, bool doLoop = false, AudioOverride? audioOverride = null)
        {
            if (_isInitialized == false)
                return;

            position ??= Vector3.zero;
            rotation ??= Quaternion.identity;
            AudioInstance instance;
            if (_pool.Count == 0 && _playingPool.Count > 0)
            {
                instance = _playingPool.Dequeue();
                instance.StopPlaying();
            }
            instance = _pool.Dequeue();
            instance.transform.position = position.Value;
            instance.transform.rotation = rotation.Value;
            instance.PlaySound(gameSound, doLoop, audioOverride);
            _playingPool.Enqueue(instance);
        }
    }
}
