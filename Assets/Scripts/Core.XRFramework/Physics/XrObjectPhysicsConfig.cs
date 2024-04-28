using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    [CreateAssetMenu]
    public class XrObjectPhysicsConfig : ScriptableObject
    {
        public float velocitySmoothing = 7f;
        public float torqueSmoothing = 7f;
        public float rotationalDelta = 4f;
    }
}
