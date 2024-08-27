using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    [CreateAssetMenu(fileName = "XrObjectPhysicsConfig", menuName = "Core/XR/Physics/Xr Object Physics Config")]
    public class XrObjectPhysicsConfig : ScriptableObject
    {
        public float maxAngularVelocity = 100f;
        public float torqueSmoothing = 7f;
        public float rotationalMultiplier = 1.5f;
    }
}
