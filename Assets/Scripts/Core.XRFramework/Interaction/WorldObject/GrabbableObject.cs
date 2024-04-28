using Core.XRFramework.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    [RequireComponent(typeof(PhysicsObject))]
    public class GrabbableObject : MonoBehaviour, IGrabbableObject
    {
        [SerializeField] private XrObjectPhysicsConfig physicsConfiguration;
        [SerializeField] private GrabPointGroup[] grabPointGroups;

        public Transform Transform => transform;

        PhysicsObject _physicsObject;

        PhysicsMover PhysicsMover => _physicsMover ??= new PhysicsMover(physicsConfiguration, _physicsObject.PhysicsRigidbody);
        PhysicsMover _physicsMover;

        bool IsTwoHanded => storedHandInformation[HandType.Right] != null && storedHandInformation[HandType.Left] != null;
        HandType _primaryGrabType;
        Dictionary<HandType, GrabPoint> storedHandInformation = new Dictionary<HandType, GrabPoint>();


        private void Awake()
        {
            _physicsObject = GetComponent<PhysicsObject>();
            storedHandInformation.Add(HandType.Left, null);
            storedHandInformation.Add(HandType.Right, null);

            foreach (var grabPointGroup in grabPointGroups)
            {
                foreach (var grabPoint in grabPointGroup.GrabPoints)
                {
                    grabPoint.parent = this;
                }
            }
        }

        public void OnHoverEnter()
        {
        }

        public void OnHoverExit()
        {
        }

        public bool TryGetGrab(HandType handType, Vector3 referencePosition, Quaternion referenceRotation, out Vector3 newPosition, out Quaternion newRotation)
        {
            newPosition = transform.position;
            newRotation = transform.rotation;
            if (grabPointGroups == null || grabPointGroups.Length == 0)
            {
                return true;
            }
            foreach (var group in grabPointGroups)
            {
                if (group.TryGetGrabPosition(handType, referencePosition, referenceRotation, out var grabPoint))
                {
                    var grabTransform = grabPoint.GetGrabTransform(referencePosition, referenceRotation);
                    storedHandInformation[handType] = grabPoint;
                    newPosition = grabTransform.newPosition;
                    newRotation = grabTransform.newRotation;
                    return true;
                }
            }
            return false;
        }

        public void GetGrabHandPosition(HandType handType, Vector3 referencePosition, Quaternion referenceRotation, out Vector3 newPosition, out Quaternion newRotation)
        {
            var getTransform = storedHandInformation[handType].GetGrabTransform(referencePosition, referenceRotation);
            newPosition = getTransform.newPosition;
            newRotation = getTransform.newRotation;
        }

        public void UpdateTransform(HandType handType, Vector3 targetPosition, Quaternion targetRotation)
        {
            if (storedHandInformation[handType] == null)
            {
                return;
            }
            if (handType != _primaryGrabType && IsTwoHanded)
            {
                return;
            }
            var getTransform = storedHandInformation[handType].GetGrabTransform(targetPosition, targetRotation);

            var calculatedTargetPosition = CalculatePositionalTarget(targetPosition, getTransform.newPosition);
            var calculatedRotation = CalculateRotationalTarget(targetRotation, getTransform.newRotation);
            _physicsObject.PhysicsRigidbody.centerOfMass = _physicsObject.PhysicsRigidbody.transform.InverseTransformPoint(getTransform.newPosition);

            PhysicsMover?.MatchTransform(calculatedTargetPosition, calculatedRotation);
        }

        Vector3 CalculatePositionalTarget(Vector3 targetPosition, Vector3 referencePosition)
        {
            Vector3 mainDifference = targetPosition - referencePosition;
            return _physicsObject.PhysicsRigidbody.position + mainDifference;
        }

        Quaternion CalculateRotationalTarget(Quaternion targetHandRotation, Quaternion targetGrabRotation)
        {
            Quaternion C = targetHandRotation * Quaternion.Inverse(targetGrabRotation);
            Quaternion D = C * _physicsObject.PhysicsRigidbody.rotation;
            return D;
        }

        public void OnGrab(HandType handType, Vector3 referencePosition, Quaternion referenceRotation)
        {
            _primaryGrabType = handType;
            TryGetGrab(handType, referencePosition, referenceRotation, out Vector3 newPosition, out Quaternion newRotation);
            _physicsObject.PhysicsRigidbody.useGravity = false; 
        }

        public void OnRelease(HandType handType, Vector3 referencePosition, Quaternion referenceRotation)
        {
            var oppositeType = handType == HandType.Left ? HandType.Right : HandType.Left;
            if (IsTwoHanded)
            {
                _primaryGrabType = oppositeType;
                _physicsObject.PhysicsRigidbody.centerOfMass = _physicsObject.PhysicsRigidbody.transform.InverseTransformPoint(storedHandInformation[oppositeType].GetGrabTransform(referencePosition, referenceRotation).newPosition);
            }
            _physicsObject.PhysicsRigidbody.useGravity = true;
            storedHandInformation[handType] = null;
            _physicsObject.PhysicsRigidbody.ResetCenterOfMass();
            _physicsMover.Reset();
        }

        public bool CanGrab()
        {
            return true;
        }
    }
}
