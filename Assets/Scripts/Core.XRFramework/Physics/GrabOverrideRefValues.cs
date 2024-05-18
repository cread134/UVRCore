using Core.XRFramework.Interaction.WorldObject;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    public struct GrabOverrideRefValues
    {
        public TransformState CurrentState;

        public Vector3 TargetPosition;
        public Quaternion TargetRotation;

        public Vector3 BodyPosition;
        public Quaternion BodyRotation;
    }
}
