using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    public class GrabPointGroup : MonoBehaviour, IInputSubscriber
    {
        #region GrabPoints

        private void Awake()
        {
            foreach (var grabPoint in GrabPoints)
            {
                grabPoint.group = this;
            }
        }

        public GrabPoint[] GrabPoints => grabPoints ??= GetComponentsInChildren<GrabPoint>();
        GrabPoint[] grabPoints;
        GrabPoint[] fLeftHandGrabPoints;
        GrabPoint[] fRightHandGrabPoints;
        GrabPoint[] RightHandGrabPoints => fRightHandGrabPoints ??= grabPoints.Where(x => x.handType == HandType.Right).ToArray();
        GrabPoint[] LeftHandGrabPoints => fLeftHandGrabPoints ??= grabPoints.Where(x => x.handType == HandType.Left).ToArray();
        GrabPoint[] GetGrabPoints(HandType handType) => handType == HandType.Left ? LeftHandGrabPoints : RightHandGrabPoints;
        #endregion

        public bool AllowTwoHandedGrab = false;

        public bool TryGetGrabPosition(HandType handType, Vector3 referencePosition, Quaternion referenceRotation, out GrabPoint grabPoint)
        {
            grabPoint = null;
            var toHandGrabPoints = GetGrabPoints(handType);
            if (grabPoints == null || toHandGrabPoints.Length == 0)
            {
                return false;
            }

            float highestPriority = 0;
            for (int i = 0; i < toHandGrabPoints.Length; i++)
            {
                var checkGrabPoint = toHandGrabPoints[i];
                if (checkGrabPoint.CanGrabPoint(referencePosition, referenceRotation, out var priority))
                {
                    if (priority > highestPriority)
                    {
                        grabPoint = checkGrabPoint;
                    }
                    Debug.DrawLine(referencePosition, checkGrabPoint.transform.position, Color.green);
                } else
                {
                    Debug.DrawLine(referencePosition, checkGrabPoint.transform.position, Color.red);
                }
            }
            if (grabPoint == null)
            {
                return false;
            }
            return true;
        }

        public void OnTriggerDown(HandType handType)
        {
        }

        public void OnTriggerUp(HandType handType)
        {
        }

        public void OnTriggerChange(HandType handType, float newValue)
        {
        }

        public void OnMainDown(HandType handType)
        {
        }

        public void OnMainUp(HandType handType)
        {
        }
    }
}
