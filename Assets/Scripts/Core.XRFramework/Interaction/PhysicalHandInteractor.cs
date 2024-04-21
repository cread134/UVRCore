using Core.XRFramework.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Interaction
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicalHandInteractor : MonoBehaviour
    {
        [SerializeField] HandType handType;
        [SerializeField] HandController handController;
        [SerializeField] XrObjectPhysicsConfig physicsConfig;

        [Header("Hand Interactor Settings")]
        [SerializeField] Transform interactionPoint;

        Collider[] Colliders => fColliders ??= GetComponentsInChildren<Collider>();
        Collider[] fColliders;

        PhysicsMover _physicsMover;
        Rigidbody _rigidbody;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rigidbody.centerOfMass = Vector3.zero;
            _physicsMover = new PhysicsMover(physicsConfig, _rigidbody);
        }

        private void FixedUpdate()
        {
            _physicsMover.MatchTransform(handController.transform);
        }

        #region Debug
        private void OnDrawGizmos()
        {
            if (interactionPoint == null || handController == null) return;
            var gripColor = Color.Lerp(Color.red, Color.green, handController.GripValue);
            Gizmos.color = gripColor;
            Gizmos.DrawSphere(interactionPoint.position, 0.01f);
        }
        #endregion
    }
}
