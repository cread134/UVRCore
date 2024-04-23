using Core.XRFramework.Interaction.WorldObject;
using Core.XRFramework.Physics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.XRFramework.Interaction
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicalHandInteractor : MonoBehaviour
    {
        [SerializeField] HandType handType;
        [SerializeField] HandController handController;

        [Header("Hand Interactor Settings")]
        [SerializeField] Transform interactionPoint;
        [SerializeField] CommonPhysicsInteractorConfig interactionConfiguration;

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
            _physicsMover = new PhysicsMover(interactionConfiguration.PhysicsConfig, _rigidbody);

            handController.OnGripPress += OnGripPress;
            handController.OnGripRelease += OnGripRelease;
        }
        #region grabbing
        IGrabbableObject hoveredObject;
        IGrabbableObject grabbedObject;
        bool fIsGrabbingObject;
        bool IsGrabbingObject => fIsGrabbingObject && grabbedObject != null;
        RaycastHit[] hoverBuffer = new RaycastHit[3];

        private void OnGripRelease(object sender, EventArgs e)
        {
        }

        private void OnGripPress(object sender, EventArgs e)
        {
        }

        private void UpdateGrabHover()
        {
            var nonAllocHits = UnityEngine.Physics.SphereCastNonAlloc(interactionPoint.position, interactionConfiguration.MaxInteractionDistance, interactionPoint.forward, hoverBuffer, interactionConfiguration.MaxInteractionDistance, interactionConfiguration.GrabbableObjectMask.LayerMask, QueryTriggerInteraction.Ignore);
            if (nonAllocHits > 0)
            {
                var closest = hoverBuffer[0];
                for (int i = 0; i < nonAllocHits; i++)
                {
                    if (hoverBuffer[i].distance < closest.distance)
                    {
                        closest = hoverBuffer[i];
                    }
                }
                hoveredObject = hoverBuffer[0].collider.GetComponent<IGrabbableObject>();
                if (hoveredObject != null)
                {
                    hoveredObject.OnHoverEnter();
                }
            }
            else
            {
                if (hoveredObject != null)
                {
                    hoveredObject.OnHoverExit();
                    hoveredObject = null;
                }
            }
        }

        private void UpdateHandTransform()
        {
            if (!IsGrabbingObject)
            {
                _physicsMover.MatchTransform(handController.transform);
            } 
            else
            {
                UpdateHelpObjectTransform();
            }
        }

        private void UpdateHelpObjectTransform()
        {

        }
        #endregion

        #region GameLoop

        private void Update()
        {
            if (!IsGrabbingObject)
            {
                UpdateGrabHover();
            }
        }

        private void FixedUpdate()
        {
            UpdateHandTransform();
        }
        #endregion

        #region Debug
        private void OnDrawGizmos()
        {
            if (!ValidateDebug()) return;
            var gripColor = Color.Lerp(Color.red, Color.green, handController.GripValue);
            Gizmos.color = gripColor;
            Gizmos.DrawWireSphere(interactionPoint.position, interactionConfiguration.MaxInteractionDistance);

            Gizmos.color = Color.blue;
            if (hoveredObject != null)
            {
                Gizmos.DrawLine(interactionPoint.position, hoveredObject.Transform.position);
            }
        }

        bool ValidateDebug()
        {
            if (interactionPoint == null || handController == null || interactionConfiguration == null)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
