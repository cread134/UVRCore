using UnityEngine;

namespace Core.Service.GameEffects
{
    public class GameParticleEffect : MonoBehaviour, IEffectInstance
    {
        [SerializeField] private ParticleSystem inner;
        public void Play()
        {
            inner.Play();
        }

        public void Stop()
        {
            inner.Stop();
        }
    }
}
