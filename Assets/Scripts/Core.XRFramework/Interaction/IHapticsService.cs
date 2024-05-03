using Core.XRFramework.Interaction;

namespace Core.XRFramework
{
    public interface IHapticsService
    {
        void SendHapticsImpulse(HandType handType, float amplitude, float duration);
    }
}