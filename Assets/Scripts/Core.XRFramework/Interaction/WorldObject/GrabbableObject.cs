using Core.Service.DependencyManagement;
using Core.Service.Logging;
using Core.Service.Physics;
using Core.XRFramework.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Core.XRFramework.Interaction.WorldObject
{
    [SelectionBase]
    [RequireComponent(typeof(PhysicsObject))]
    [RequireComponent(typeof(NetworkObject))]
    public class GrabbableObject : MonoBehaviour, IGrabbableObject, IInputSubscriber
    {
        #region Networking

        readonly NetworkVariable<bool> _isBeingGrabbed = new NetworkVariable<bool>();
        readonly NetworkVariable<ulong> _ownerId = new NetworkVariable<ulong>();
        
        public ulong OwnerId => _ownerId.Value;

        public void SetGrabbedServerCmd(ulong id, bool value)
        {
            _isBeingGrabbed.Value = value;
            _ownerId.Value = id;
            //change ownership
            if (value)
            {
                GetComponent<NetworkObject>().ChangeOwnership(id);
            } 
            else
            {
                GetComponent<NetworkObject>().ChangeOwnership(0);
            }
        }

        #endregion

        private GrabPointGroup[] grabPointGroups = new GrabPointGroup[0];

        public Transform Transform => transform;
        public PhysicsObject PhysicsObject => _physicsObject;
        PhysicsObject _physicsObject;

        LazyService<ILoggingService> loggingService = new LazyService<ILoggingService>();

        public bool IsTwoHanded => storedHandInformation[HandType.Right].IsGrabbing && storedHandInformation[HandType.Left].IsGrabbing;
        public bool IsBeingGrabbed { get; private set; }
        public bool IsHovered { get; private set; }

        HandType _primaryGrabType;
        Dictionary<HandType, CachedHandInformation> storedHandInformation = new Dictionary<HandType, CachedHandInformation>();

        [HideInInspector]
        public EventHandler ReleaseRequested { get; set; }

        [SerializeField] private LayerMaskConfiguration inputMask;
        [SerializeField] private float socketInputRadius = 0.1f;

        private void Awake()
        {
            SearchForGrabPointGroups();
            _physicsObject = GetComponent<PhysicsObject>();
            storedHandInformation.Add(HandType.Left, new(HandType.Left));
            storedHandInformation.Add(HandType.Right, new(HandType.Right));

            foreach (var grabPointGroup in grabPointGroups)
            {
                foreach (var grabPoint in grabPointGroup.GrabPoints)
                {
                    grabPoint.Parent = this;
                }
            }
        }

        void SearchForGrabPointGroups()
        {
            grabPointGroups = GetComponentsInChildren<GrabPointGroup>();
        }

        public void OnHoverEnter()
        {
            IsHovered = true;
        }

        public void OnHoverExit()
        {
            IsHovered = false;
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
                    storedHandInformation[handType].GrabPoint = grabPoint;
                    storedHandInformation[handType].TargetUpVector = referenceUp;
                    storedHandInformation[handType].TargetPosition = referencePosition;
                    storedHandInformation[handType].TargetRotation = referenceRotation;
                    var grabTransform = storedHandInformation[handType].GetGrabTransform();
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
            var targetPosition = cachedInformation.TargetPosition;
            var targetRotation = cachedInformation.TargetRotation;

            var oppositeSide = _primaryGrabType != HandType.Left ? HandType.Left : HandType.Right;
            var mainHandTarget = storedHandInformation[oppositeSide].TargetPosition;

            Vector3 calculatedTargetPosition;
            Quaternion calculatedRotation;
            if (IsTransformOverriden)
            {
                var newValues = OnGrabOverrideUpdate(cachedInformation.TargetPosition,
                                                     cachedInformation.TargetRotation,
                                                     _physicsObject.PhysicsRigidbody.position,
                                                     _physicsObject.PhysicsRigidbody.rotation,
                                                     getTransform);
                calculatedTargetPosition = newValues.newTarget;
                calculatedRotation = newValues.newTargetRotation;
            }
            else
            {
                calculatedTargetPosition = CalculatePositionalTarget(targetPosition, getTransform.Position);
                calculatedRotation = ShouldApplyTwoHandedRotation() ? CalculateTwoHandedRotation(mainHandTarget, oppositeSide)
                                                                    : CalculateRotationalTarget(targetRotation, getTransform.Rotation);
            }

            PhysicsObject.Match(calculatedTargetPosition, calculatedRotation);
        }

        #region override

        (Vector3 newTarget, Quaternion newTargetRotation) OnGrabOverrideUpdate(Vector3 currentTargetPos, Quaternion currentTargetRotation, Vector3 bodyPosition, Quaternion bodyRotation, TransformState currentState)
        {
            return _grabOverride.GetOverrideTransform(new GrabOverrideRefValues
            {
                CurrentState = currentState,
                TargetPosition = currentTargetPos,
                TargetRotation = currentTargetRotation,
                BodyPosition = bodyPosition,
                BodyRotation = bodyRotation
            });
        }

        public bool IsTransformOverriden { get; private set; }
        bool overrideDisableCollision = false; 
        IGrabOverrider _grabOverride;
        public void SetOverride(IGrabOverrider grabOverride, bool disableCollision = false)
        {
            _grabOverride = grabOverride;
            overrideDisableCollision = disableCollision;

            IsTransformOverriden = true;
            if (disableCollision)
            {
                _physicsObject.CollisionActive = false;
            }
        }

        public void ReleaseOverride()
        {
            _grabOverride = null;
            IsTransformOverriden = false;
            if (overrideDisableCollision)
            {
                _physicsObject.CollisionActive = true;
            }
        }

        #endregion

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
            if (!IsTwoHanded)
            {
                return false;
            }
            var groups = new GrabPointGroup[]
            {
                storedHandInformation[HandType.Left].GrabPoint.Group,
                storedHandInformation[HandType.Right].GrabPoint.Group
            };
            if (groups.Any(x => !x.AllowTwoHandedMovement))
            {
                return false;
            }
            return (groups[0] != groups[1] || storedHandInformation[_primaryGrabType].GrabPoint.Group.AllowTwoHandedGrab);
        }

        Quaternion CalculateTwoHandedRotation(Vector3 mainGrabTargetPosiiton, HandType oppositeSide)
        {
            //two handed movement
            var mainGrabPoint = storedHandInformation[_primaryGrabType].GetGrabTransform();
            var secondaryGrabPoint = storedHandInformation[oppositeSide].GetGrabTransform();

            var betweenVector = secondaryGrabPoint.Position - mainGrabPoint.Position;
            var mainGrabMagnitude = betweenVector.magnitude;
            var targetVector = (mainGrabTargetPosiiton - mainGrabPoint.Position).normalized * mainGrabMagnitude;

            const float mainHandUpInfluence = 0.3f;
            var upDirection = Vector3.Lerp(storedHandInformation[_primaryGrabType].TargetUpVector, storedHandInformation[oppositeSide].TargetUpVector, mainHandUpInfluence);
            var dif = Quaternion.LookRotation(targetVector, upDirection) * Quaternion.Inverse(Quaternion.LookRotation(betweenVector, mainGrabPoint.UpDirection));
            var resultant = dif * transform.rotation;
            Debug.DrawLine(mainGrabPoint.Position, mainGrabPoint.Position + targetVector, Color.red);
            return resultant;
        }

        public void OnGrab(HandType handType, Vector3 referencePosition, Vector3 referenceUp, Quaternion referenceRotation)
        {
            IsBeingGrabbed = true;
            loggingService.Value.Log($"object grabbed with {handType} hand", context: this);
            var oppositeType = handType == HandType.Left ? HandType.Right : HandType.Left;
            if (storedHandInformation[oppositeType].IsGrabbing 
             && storedHandInformation[oppositeType].GrabPoint.priority >= storedHandInformation[handType].GrabPoint.priority)
            {
                _primaryGrabType = oppositeType;
            } else
            {
                _primaryGrabType = handType;
            }
            TryGetGrab(handType, referencePosition, referenceUp, referenceRotation, out Vector3 newPosition, out Quaternion newRotation);

            storedHandInformation[handType].IsGrabbing = true;
            _physicsObject.PhysicsRigidbody.useGravity = false;
            _physicsObject.PhysicsRigidbody.centerOfMass = newPosition - _physicsObject.PhysicsRigidbody.position;
        }

        public void OnRelease(HandType handType, Vector3 referencePosition, Quaternion referenceRotation)
        {
            IsBeingGrabbed = false;
            var oppositeType = handType == HandType.Left ? HandType.Right : HandType.Left;
            if (IsTwoHanded)
            {
                _primaryGrabType = oppositeType;
                _physicsObject.PhysicsRigidbody.centerOfMass = storedHandInformation[oppositeType].GetGrabTransform().Position - _physicsObject.PhysicsRigidbody.position;
            }
            _physicsObject.PhysicsRigidbody.useGravity = true;
            storedHandInformation[handType].IsGrabbing = false;
            _physicsObject.ResetCentreOfMass();

            PostRelease();
        }

        void PostRelease()
        {
            if(CheckForSockets())
            {
                return;
            }
        }

        bool CheckForSockets()
        {
            var colliders = UnityEngine.Physics.OverlapSphere(transform.position, socketInputRadius, inputMask.LayerMask, QueryTriggerInteraction.Collide);
            foreach (var collider in colliders)
            {
                if (collider.TryGetComponent(out IObjectSocket socket))
                {
                    if (socket.CanHoldObject(this))
                    {
                        socket.AttachObject(this);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CanGrab(ulong clientId)
        {
            if (IsBeingGrabbed && _ownerId.Value != clientId)
            {
                return false;
            }
            return true;
        }

        public void DoRelease()
        {
            if (IsBeingGrabbed)
            {
                ReleaseRequested?.Invoke(this, EventArgs.Empty);
            }
        }

        #region inputTransmission

        public void OnTriggerDown(HandType handType)
        {
            var handInformation = storedHandInformation[handType];
            if (handInformation.IsGrabbing && handInformation.GrabPoint.Group is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnTriggerDown(handType);
            }
        }

        public void OnTriggerUp(HandType handType)
        {
            var handInformation = storedHandInformation[handType];
            if (handInformation.IsGrabbing && handInformation.GrabPoint.Group is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnTriggerUp(handType);
            }
        }

        public void OnTriggerChange(HandType handType, float newValue)
        {
            var handInformation = storedHandInformation[handType];
            if (handInformation.IsGrabbing && handInformation.GrabPoint.Group is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnTriggerChange(handType, newValue);
            }
        }

        public void OnMainDown(HandType handType)
        {
            var handInformation = storedHandInformation[handType];
            if (!handInformation.IsGrabbing && handInformation.GrabPoint.Group is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnMainDown(handType);
            }
        }

        public void OnMainUp(HandType handType)
        {
            var handInformation = storedHandInformation[handType];
            if (!handInformation.IsGrabbing && handInformation.GrabPoint.Group is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnMainUp(handType);
            }
        }
        #endregion
    }
}