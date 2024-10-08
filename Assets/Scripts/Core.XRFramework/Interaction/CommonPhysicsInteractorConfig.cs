using Core.Service.Physics;
using Core.XRFramework.Physics;
using UnityEngine;

namespace Core.XRFramework.Interaction
{
    [CreateAssetMenu]
    internal class CommonPhysicsInteractorConfig : ScriptableObject
    {
        public float MaxInteractionDistance = 0.015f;
        public XrObjectPhysicsConfig PhysicsConfig;
        public LayerMaskConfiguration GrabbableObjectMask;

        public float hapticGrabAmplitude = 1.0f;
        public float hapticGrabDuration = 0.5f;
    }
}
