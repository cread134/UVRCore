using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    public class CachedHandInformation
    {
        public CachedHandInformation(HandType handType)
        {
            HandType = handType
                ;
        }
        public Vector3 TargetPosition;
        public Quaternion TargetRotation;
        public Vector3 TargetUpVector;
        public GrabPoint GrabPoint;
        public HandType HandType;

        bool fIsGrabbing;
        public bool IsGrabbing
        {
            get 
            { 
                return fIsGrabbing; 
            }
            set
            {
                fIsGrabbing = value;
                if (GrabPoint != null)
                {
                    GrabPoint.IsGrabbed = value;
                    GrabPoint.handType = HandType;
                    GrabPoint.OnGrabbed(HandType, TargetPosition, TargetRotation);
                }
            }
        }

        internal TransformState GetGrabTransform()
        {
            return GrabPoint.GetGrabTransform(TargetPosition, TargetUpVector, TargetRotation);
        }
    }
}
