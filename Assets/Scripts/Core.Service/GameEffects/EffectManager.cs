using Core.Service.Pooling;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Service.GameEffects
{
    internal class EffectManager : MonoBehaviour, IEffectManager
    {
        Dictionary<GameEffect, PriorityPool> effectPools = new Dictionary<GameEffect, PriorityPool>();

        public IEffectInstance PlayEffect(GameEffect effect, Vector3 position, Quaternion rotation)
        {
            if (!effectPools.ContainsKey(effect))
            {
                effectPools.Add(effect, new PriorityPool(effect.EffectPrefab, transform));
            }

            var effectInstance = effectPools[effect];
            var instance = effectInstance.GetInstance();
            instance.transform.position = position;
            instance.transform.rotation = rotation;

            var effectInstanceComponent = instance.GetComponent<IEffectInstance>();
            effectInstanceComponent?.Play();
            return effectInstanceComponent;
        }
    }
}
