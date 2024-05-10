using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    public class CachedHandInformation
    {
        public CachedHandInformation(HandType handType)
        {
            HandType = handType;
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
  
                    if (value)
                    {
                        GrabPoint.handType = HandType;
                        GrabPoint.OnGrabbed(HandType, TargetPosition, TargetRotation);
                    }
                    else
                    {
                        GrabPoint.OnReleased(HandType);
                    }

                    if (GrabPoint.Group != null)
                    {
                        GrabPoint.Group.IsGrabbed = value;
                        if (value)
                        {
                            GrabPoint.Group.OnGrabbed(HandType, GrabPoint, TargetPosition, TargetRotation);
                        }
                        else
                        {
                            GrabPoint.Group.OnReleased(HandType, GrabPoint);
                        }
                    }
                }
            }
        }

        internal TransformState GetGrabTransform()
        {
            return GrabPoint.Group.GetGrabTransform(TargetPosition, TargetUpVector, TargetRotation, GrabPoint);
        }
    }
}
