using Core.Service.DependencyManagement;
using Core.Service.Logging;
using Core.XRFramework.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    [SelectionBase]
    [RequireComponent(typeof(PhysicsObject))]
    public class GrabbableObject : MonoBehaviour, IGrabbableObject, IInputSubscriber
    {
        [SerializeField] private XrObjectPhysicsConfig physicsConfiguration;
        [SerializeField] private GrabPointGroup[] grabPointGroups;

        public Transform Transform => transform;

        PhysicsObject _physicsObject;

        PhysicsMover PhysicsMover => _physicsMover ??= new PhysicsMover(physicsConfiguration, _physicsObject.PhysicsRigidbody);
        PhysicsMover _physicsMover;

        ILoggingService _loggingService;
        ILoggingService LoggingService => _loggingService ??= ObjectFactory.ResolveService<ILoggingService>();

        bool IsTwoHanded => storedHandInformation[HandType.Right].IsGrabbing && storedHandInformation[HandType.Left].IsGrabbing;
        HandType _primaryGrabType;
        Dictionary<HandType, CachedHandInformation> storedHandInformation = new Dictionary<HandType, CachedHandInformation>();

        private void Awake()
        {
            _physicsObject = GetComponent<PhysicsObject>();
            storedHandInformation.Add(HandType.Left, new());
            storedHandInformation.Add(HandType.Right, new());

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

        public bool TryGetGrab(HandType handType, Vector3 referencePosition, Vector3 referenceUp, Quaternion referenceRotation, out Vector3 newPosition, out Quaternion newRotation)
        {
            newPosition = transform.position;
            newRotation = transform.rotation;
            if (grabPointGroups == null || grabPointGroups.Length == 0)
            {
                return true;
            }
            foreach (var group in grabPointGroups)
            {
                Debug.DrawLine(referencePosition, group.transform.position, Color.blue);
                if (group.TryGetGrabPosition(handType, referencePosition, referenceRotation, out var grabPoint))
                {
                    var grabTransform = grabPoint.GetGrabTransform(referencePosition, referenceUp, referenceRotation);
                    storedHandInformation[handType].GrabPoint = grabPoint;
                    newPosition = grabTransform.Position;
                    newRotation = grabTransform.Rotation;
                    return true;
                }
            }
            return false;
        }

        public void GetGrabHandPosition(HandType handType, out Vector3 newPosition, out Quaternion newRotation)
        {
            var getTransform = storedHandInformation[handType].GetGrabTransform();
            newPosition = getTransform.Position;
            newRotation = getTransform.Rotation;
        }

        public void UpdateTransformState(HandType handType)
        {
            if (!storedHandInformation[handType].IsGrabbing)
            {
                return;
            }
            if (handType != _primaryGrabType && IsTwoHanded)
            {
                return;
            }
            var cachedInformation = storedHandInformation[handType];
            var getTransform = cachedInformation.GetGrabTransform();

            var calculatedTargetPosition = CalculatePositionalTarget(cachedInformation.TargetPosition, getTransform.Position);
            var calculatedRotation = ShouldApplyTwoHandedRotation() ? CalculateTwoHandedRotation() 
                                                                    : CalculateRotationalTarget(cachedInformation.TargetRotation, getTransform.Rotation);

            PhysicsMover?.MatchTransform(calculatedTargetPosition, calculatedRotation, _physicsObject);
        }

        public void UpdateCachedValues(HandType handType, Vector3 targetPosition, Vector3 upDirection, Quaternion targetRotation)
        {
            storedHandInformation[handType].TargetUpVector = upDirection;
            storedHandInformation[handType].TargetPosition = targetPosition;
            storedHandInformation[handType].TargetRotation = targetRotation;
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

        bool ShouldApplyTwoHandedRotation()
        {
            return IsTwoHanded 
                && (storedHandInformation[HandType.Left].GrabPoint.group != storedHandInformation[HandType.Right].GrabPoint.group || storedHandInformation[_primaryGrabType].GrabPoint.group.AllowTwoHandedGrab);
        }

        Quaternion CalculateTwoHandedRotation()
        {
            //two handed movement
            var mainGrabPoint = storedHandInformation[_primaryGrabType].GetGrabTransform();
            var oppositeSide = _primaryGrabType != HandType.Left ? HandType.Left : HandType.Right;
            var secondaryGrabPoint = storedHandInformation[oppositeSide].GetGrabTransform();
            var mainGrabTargetPosiiton = storedHandInformation[oppositeSide].TargetPosition;

            var betweenVector = secondaryGrabPoint.Position - mainGrabPoint.Position;
            var mainGrabMagnitude = betweenVector.magnitude;
            var targetVector = (mainGrabTargetPosiiton - mainGrabPoint.Position).normalized * mainGrabMagnitude;
            const float mainHandUpInfluence = 0.3f;
            var upDirection = Vector3.Lerp(storedHandInformation[_primaryGrabType].TargetUpVector, storedHandInformation[oppositeSide].TargetUpVector, mainHandUpInfluence);
            var dif = Quaternion.LookRotation(targetVector, upDirection) * Quaternion.Inverse(Quaternion.LookRotation(betweenVector, mainGrabPoint.UpDirection));
            var restulant = dif * transform.rotation;

            return restulant;
        }

        public void OnGrab(HandType handType, Vector3 referencePosition, Vector3 referenceUp, Quaternion referenceRotation)
        {
            LoggingService.Log($"object grabbed with {handType} hand", context: this);
            var oppositeType = handType == HandType.Left ? HandType.Right : HandType.Left;
            if (storedHandInformation[oppositeType].IsGrabbing)
            {
                _primaryGrabType = oppositeType;
            } else
            {
                _primaryGrabType = handType;
            }
            storedHandInformation[handType].IsGrabbing = true;
            TryGetGrab(handType, referencePosition, referenceUp, referenceRotation, out Vector3 newPosition, out Quaternion newRotation);
            _physicsObject.PhysicsRigidbody.useGravity = false;
            _physicsObject.PhysicsRigidbody.centerOfMass = newPosition - _physicsObject.PhysicsRigidbody.position;
        }

        public void OnRelease(HandType handType, Vector3 referencePosition, Quaternion referenceRotation)
        {
            var oppositeType = handType == HandType.Left ? HandType.Right : HandType.Left;
            if (IsTwoHanded)
            {
                _primaryGrabType = oppositeType;
                _physicsObject.PhysicsRigidbody.centerOfMass = storedHandInformation[oppositeType].GetGrabTransform().Position - _physicsObject.PhysicsRigidbody.position;
            }
            _physicsObject.PhysicsRigidbody.useGravity = true;
            storedHandInformation[handType].IsGrabbing = false;
            _physicsObject.ResetCentreOfMass();
            _physicsMover.Reset();
        }

        public bool CanGrab()
        {
            return true;
        }

        #region inputTransmission

        public void OnTriggerDown(HandType handType)
        {
            var handInformation = storedHandInformation[handType];
            if (handInformation.IsGrabbing && handInformation.GrabPoint.group is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnTriggerDown(handType);
            }
        }

        public void OnTriggerUp(HandType handType)
        {
            var handInformation = storedHandInformation[handType];
            if (handInformation.IsGrabbing && handInformation.GrabPoint.group is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnTriggerUp(handType);
            }
        }

        public void OnTriggerChange(HandType handType, float newValue)
        {
            var handInformation = storedHandInformation[handType];
            if (handInformation.IsGrabbing && handInformation.GrabPoint.group is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnTriggerChange(handType, newValue);
            }
        }

        public void OnMainDown(HandType handType)
        {
            var handInformation = storedHandInformation[handType];
            if (!handInformation.IsGrabbing && handInformation.GrabPoint.group is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnMainDown(handType);
            }
        }

        public void OnMainUp(HandType handType)
        {
            var handInformation = storedHandInformation[handType];
            if (!handInformation.IsGrabbing && handInformation.GrabPoint.group is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnMainUp(handType);
            }
        }
        #endregion
    }
}