using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Core.XRFramework.Interaction.WorldObject
{
    public class GrabPointGroup : MonoBehaviour, IInputSubscriber
    {
        #region GrabPoints

        private void Awake()
        {
            foreach (var grabPoint in GrabPoints)
            {
                grabPoint.Group = this;
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
                    Debug.DrawLine(referencePosition, checkGrabPoint.transform.position, Color.magenta);
                } else
                {
                    Debug.DrawLine(referencePosition, checkGrabPoint.transform.position, Color.red);
                }
            }
            if (grabPoint == null)
            {
                return false;
            }
            Debug.DrawLine(referencePosition, grabPoint.transform.position, Color.green);
            return true;
        }

        [Header("Input events")]
        public UnityEvent<HandType, GrabPoint> OnTriggerDownEvent = new();
        public UnityEvent<HandType, GrabPoint> OnTriggerUpEvent = new();
        public UnityEvent<HandType, GrabPoint, float> OnTriggerChangeEvent = new();
        public UnityEvent<HandType, GrabPoint> OnMainDownEvent = new();
        public UnityEvent<HandType, GrabPoint> OnMainUpEvent = new();

        GrabPoint GetGrabbedPoint(HandType handType)
        {
            return grabPoints.FirstOrDefault(x => x.IsGrabbed && x.handType == handType);
        }

        public void OnTriggerDown(HandType handType)
        {
            var grabbedPoint = GetGrabbedPoint(handType);
            if (grabbedPoint != null)
            {
                OnTriggerDownEvent.Invoke(handType, grabbedPoint);
            }
        }

        public void OnTriggerUp(HandType handType)
        {
            var grabbedPoint = GetGrabbedPoint(handType);
            if (grabbedPoint != null)
            {
                OnTriggerUpEvent.Invoke(handType, grabbedPoint);
            }
        }

        public void OnTriggerChange(HandType handType, float newValue)
        {
            var grabbedPoint = GetGrabbedPoint(handType);
            if (grabbedPoint != null)
            {
                OnTriggerChangeEvent.Invoke(handType, grabbedPoint, newValue);
            }
        }

        public void OnMainDown(HandType handType)
        {
            var grabbedPoint = GetGrabbedPoint(handType);
            if (grabbedPoint != null)
            {
                OnMainDownEvent.Invoke(handType, grabbedPoint);
            }
        }

        public void OnMainUp(HandType handType)
        {
            var grabbedPoint = GetGrabbedPoint(handType);
            if (grabbedPoint != null)
            {
                OnMainUpEvent.Invoke(handType, grabbedPoint);
            }
        }
    }
}
