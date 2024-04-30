using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    public class CachedHandInformation
    {
        public Vector3 TargetPosition;
        public Quaternion TargetRotation;
        public Vector3 TargetUpVector;
        public GrabPoint GrabPoint;
        public bool IsGrabbing;

        internal TransformState GetGrabTransform()
        {
            return GrabPoint.GetGrabTransform(TargetPosition, TargetUpVector, TargetRotation);
        }
    }
}
