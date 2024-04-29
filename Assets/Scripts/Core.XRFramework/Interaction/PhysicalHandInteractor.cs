using Core.XRFramework.Context;
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
        [SerializeField] GrabInteractionIcon grabIndicator;
        [SerializeField] Transform interactionPoint;
        [SerializeField] CommonPhysicsInteractorConfig interactionConfiguration;

        GameObject[] LayerObjects
        {
            get
            {
                if (fLayerObjects == null)
                {
                    var foundGameObjects = new HashSet<GameObject>();
                    var colliders = GetComponentsInChildren<Collider>();
                    foreach (var collider in colliders)
                    {
                        foundGameObjects.Add(collider.gameObject);
                    }
                    fLayerObjects = foundGameObjects.ToArray();
                }
                return fLayerObjects;
            }
        }

        GameObject[] fLayerObjects;
        PhysicsMover _physicsMover;
        Rigidbody _rigidbody;
        XrContext _xrContext;
        Camera _camera;

        private void Awake()
        {
            _xrContext = FindObjectOfType<XrContext>() ?? throw new Exception("No XR Context found in scene");
            _camera = _xrContext.GetCamera();

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rigidbody.centerOfMass = Vector3.zero;
            _physicsMover = new PhysicsMover(interactionConfiguration.PhysicsConfig, _rigidbody);

            handController.OnGripPress += OnGripPress;
            handController.OnGripRelease += OnGripRelease;

            grabIndicator.SetActive(false);
        }
        #region grabbing
        public IGrabbableObject hoveredObject;
        IGrabbableObject grabbedObject;
        bool _isGrabbingObject;
        bool IsGrabbingObject => _isGrabbingObject && grabbedObject != null;
        bool CanGrabObject => hoveredObject != null && hoveredObject.CanGrab();

        RaycastHit[] hoverBuffer = new RaycastHit[3];

        #region input
        private void OnGripRelease(object sender, EventArgs e)
        {
            TryRelease();
        }

        private void OnGripPress(object sender, EventArgs e)
        {
            TryGrab();
        }
        #endregion

        public void TryGrab()
        {
            if (!IsGrabbingObject && CanGrabObject)
            {
                GrabObject();
            }
        }

        public void TryRelease()
        {
            if (IsGrabbingObject)
            {
                ReleaseObject();
            }
        }

        private void GrabObject()
        {
            grabbedObject = hoveredObject;
            hoveredObject = null;
            _isGrabbingObject = true;
            grabbedObject.OnGrab(handType, interactionPoint.position, _rigidbody.rotation);
            _rigidbody.isKinematic = true;
            grabIndicator.SetActive(false);
            SetCollisionActive(false);

            HapticsService.Instance?.SendHapticsImpulse(handType, interactionConfiguration.hapticGrabAmplitude, interactionConfiguration.hapticGrabDuration);
        }

        private void ReleaseObject()
        {
            _isGrabbingObject = false;
            grabbedObject.OnRelease(handType, interactionPoint.position, _rigidbody.rotation);
            _rigidbody.isKinematic = false;
            grabIndicator.SetActive(false);
            SetCollisionActive(true);
            grabbedObject = null;
            hoveredObject = null;
            _physicsMover.Reset();

            HapticsService.Instance?.SendHapticsImpulse(handType, interactionConfiguration.hapticGrabAmplitude, interactionConfiguration.hapticGrabDuration);
        }

        void SetCollisionActive(bool active)
        {
            var targetLayer = active ? 7 : 8;
            foreach (var layerObject in LayerObjects)
            {
                layerObject.layer = targetLayer;
            }
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

                if (hoveredObject.TryGetGrab(handType, _rigidbody.position, _rigidbody.rotation, out var newPosition, out var newRotation))
                {
                    grabIndicator.SetActive(true);
                    var lookToCameraDirection = _camera.transform.position - newPosition;
                    grabIndicator.UpdateTransform(newPosition, _rigidbody.position, Quaternion.LookRotation(lookToCameraDirection, Vector3.up));
                } 
                else
                {
                    grabIndicator.SetActive(false);
                    hoveredObject = null;
                }
            }
            else
            {
                if (hoveredObject != null)
                {
                    hoveredObject.OnHoverExit();
                    hoveredObject = null;
                    grabIndicator.SetActive(false);
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
            grabbedObject.UpdateCachedValues(handType, handController.transform.position, handController.transform.rotation);
            grabbedObject.UpdateTransformState(handType);
            grabbedObject.GetGrabHandPosition(handType, out var newPosition, out var newRotation);
            _rigidbody.MovePosition(newPosition);
            _rigidbody.MoveRotation(newRotation);
           
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
