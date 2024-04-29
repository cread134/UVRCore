using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    public class CachedHandInformation
    {
        public Vector3 TargetPosition;
        public Quaternion TargetRotation;
        public GrabPoint GrabPoint;
        public bool IsGrabbing;

        internal (Vector3 newPosition, Quaternion newRotation) GetGrabTransform()
        {
            return GrabPoint.GetGrabTransform(TargetPosition, TargetRotation);
        }
    }
}
