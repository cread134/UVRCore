using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    [CreateAssetMenu]
    public class XrObjectPhysicsConfig : ScriptableObject
    {
        public float positionDifferenceMatchForce = 4f;
        public float positionErrorMatchForce = 8f;
        public float positionSmoothing = 7f;
        public float positionVelocityMultipler = 2f;
        public float positionIntegrationCompenstation = 1f;
        [Space]
        public float rotationProportionalGain = 8f; // Proportional gain for rotation control
        public float torqueDamping = 1f;
        public float rotationErrorCompenstation = 8f;
        public float angleAllowance = 5f;
        public float rotationalSmoothing = 12f;
        public float rotationImpulseCompensation = 1f;
        public float rotationIntergral = 1.5f;
        [Range(0f, 1f)] public float anglularSlowdown = 0.7f;
    }
}
