using Core.XRFramework.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    [RequireComponent(typeof(PhysicsObject))]
    internal class GrabbableObject : MonoBehaviour, IGrabbableObject
    {
        [SerializeField] private XrObjectPhysicsConfig physicsConfiguration;

        public Transform Transform => transform;

        PhysicsObject _physicsObject;

        PhysicsMover PhysicsMover => _physicsMover ??= new PhysicsMover(physicsConfiguration, _physicsObject.PhysicsRigidbody);
        PhysicsMover _physicsMover;

        private void Awake()
        {
            _physicsObject = GetComponent<PhysicsObject>();
        }

        public void OnHoverEnter()
        {
        }

        public void OnHoverExit()
        {
        }

        public bool TryGetGrab(Vector3 referencePosition, Quaternion referenceRotation, out Vector3 newPosition, out Quaternion newRotation)
        {
            GetGrabPosition(referencePosition, referenceRotation, out newPosition, out newRotation);
            return true;
        }

        public void GetGrabPosition(Vector3 referencePosition, Quaternion referenceRotation, out Vector3 newPosition, out Quaternion newRotation)
        {
            newPosition = transform.position;
            newRotation = transform.rotation;
        }

        public void UpdateTransform(Vector3 targetPosition, Quaternion targetRotation)
        {
            PhysicsMover?.MatchTransform(targetPosition, targetRotation);
        }

        public void OnGrab(Vector3 referencePosition, Quaternion referenceRotation)
        {
            _physicsObject.PhysicsRigidbody.useGravity = false;
        }

        public void OnRelease(Vector3 referencePosition, Quaternion referenceRotation)
        {
            _physicsObject.PhysicsRigidbody.useGravity = true;
        }

        public bool CanGrab()
        {
            return true;
        }
    }
}
