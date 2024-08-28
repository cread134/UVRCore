using UnityEngine;

namespace Core.Service.GameEffects
{
    public interface IEffectManager
    {
        IEffectInstance PlayEffect(GameEffect effect, Vector3 position, Quaternion rotation);
    }
}
