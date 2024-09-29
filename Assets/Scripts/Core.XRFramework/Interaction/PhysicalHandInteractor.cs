using Core.Service.DependencyManagement;
using Core.Service.ObjectManagement;
using Core.Service.Physics;
using Core.XRFramework.Context;
using Core.XRFramework.Interaction.WorldObject;
using Core.XRFramework.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Core.XRFramework.Interaction
{
    [SelectionBase, RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(PhysicsObject))]
    public class PhysicalHandInteractor : NetworkBehaviour, ISpawnable
    {
        [SerializeField] HandType handType;
        [SerializeField] HandController handController;

        [Header("Hand Interactor Settings")]
        [SerializeField] GrabInteractionIcon grabIndicator;
        [SerializeField] Transform interactionPoint;
        [SerializeField] CommonPhysicsInteractorConfig interactionConfiguration;

        PhysicsObject _physicsObject;

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

        LazyService<IHapticsService> hapticsService = new ();

        bool hasSpawned = false;
        public void Spawned()
        {
            _xrContext = FindFirstObjectByType<XrContext>() ?? throw new Exception("No XR Context found in scene");
            _camera = _xrContext.GetCamera();

            _rigidbody = GetComponent<Rigidbody>();
            _physicsObject = GetComponent<PhysicsObject>();
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            _rigidbody.centerOfMass = Vector3.zero;
            _physicsMover = new PhysicsMover(interactionConfiguration.PhysicsConfig, _rigidbody);

            handController.OnGripPress += OnGripPress;
            handController.OnGripRelease += OnGripRelease;

            handController.OnTriggerPress += OnTriggerPressed;
            handController.OnTriggerRelease += OnTriggerReleased;
            handController.OnTriggerChangeEvent += OnTriggerChanged;

            handController.OnMainButtonDownEvent += OnMainPressed;
            handController.OnSecondaryButtonUpEvent += OnMainReleased;

            grabIndicator.SetActive(false);
            hasSpawned = true;
        }

        #region GameLoop

        private void Update()
        {
            if (!hasSpawned || !IsOwner)
            {
                return;
            }
            if (!IsGrabbingObject)
            {
                UpdateGrabHover();
            }
        }

        private void FixedUpdate()
        {
            if (!hasSpawned || !IsOwner)
            {
                return;
            }
            UpdateHandTransform();
        }

        #endregion

        #region grabbing
        public IGrabbableObject hoveredObject;
        IGrabbableObject grabbedObject;
        bool _isGrabbingObject;
        bool IsGrabbingObject => _isGrabbingObject && grabbedObject != null;
        bool CanGrabObject => hoveredObject != null && hoveredObject.CanGrab(OwnerClientId);

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

        public void OnReleaseRequested(object grabbableObject, EventArgs e)
        {
            if (grabbedObject == grabbableObject)
            {
                TryRelease();
            }
        }

        #region Grab and Release
        private void GrabObject()
        {
            grabbedObject = hoveredObject;
            grabbedObject.ReleaseRequested += OnReleaseRequested;

            hoveredObject = null;
            _isGrabbingObject = true;
            grabbedObject.OnGrab(handType, interactionPoint.position, interactionPoint.up, _rigidbody.rotation);
            if (grabbedObject is NetworkBehaviour networkBehaviour)
            {
                ServerRequestGrabServerRpc(new NetworkObjectReference(networkBehaviour.NetworkObject), OwnerClientId, true);
            }
            _rigidbody.isKinematic = true;
            grabIndicator.SetActive(false);
            SetCollisionActive(false);

            hapticsService.Value?.SendHapticsImpulse(handType, interactionConfiguration.hapticGrabAmplitude, interactionConfiguration.hapticGrabDuration);
        }

        private void ReleaseObject()
        {
            grabbedObject.ReleaseRequested -= OnReleaseRequested;

            _isGrabbingObject = false;
            grabbedObject.OnRelease(handType, interactionPoint.position, _rigidbody.rotation);
            if (grabbedObject is NetworkBehaviour networkBehaviour)
            {
                ServerRequestGrabServerRpc(new NetworkObjectReference(networkBehaviour.NetworkObject), OwnerClientId, false);
            }
            _rigidbody.isKinematic = false;
            grabIndicator.SetActive(false);
            SetCollisionActive(true);
            grabbedObject = null;
            hoveredObject = null;
            _physicsMover.Reset();

            hapticsService.Value?.SendHapticsImpulse(handType, interactionConfiguration.hapticGrabAmplitude, interactionConfiguration.hapticGrabDuration);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ServerRequestGrabServerRpc(NetworkObjectReference objectReference, ulong clientId, bool isGrab)
        {
            if (objectReference.TryGet(out var grabbableObjectBase))
            {
                if (grabbableObjectBase.gameObject.TryGetComponent(out IGrabbableObject grabbableObject))
                {
                    grabbedObject.SetGrabbedServerCmd(clientId, isGrab);
                }
            }
        }
        #endregion

        void SetCollisionActive(bool active)
        {
            var targetLayer = active ? LayerInfo.PlayerHands : LayerInfo.PlayerHandsHeld;
            foreach (var layerObject in LayerObjects)
            {
                layerObject.layer = (int)targetLayer;
            }
        }

        private void UpdateGrabHover()
        {
            var nonAllocHits = UnityEngine.Physics.SphereCastNonAlloc(interactionPoint.position, interactionConfiguration.MaxInteractionDistance, interactionPoint.forward, hoverBuffer, interactionConfiguration.MaxInteractionDistance / 2f, interactionConfiguration.GrabbableObjectMask.LayerMask, QueryTriggerInteraction.Ignore);
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
                hoveredObject = hoverBuffer[0].collider.attachedRigidbody?.GetComponent<IGrabbableObject>();
                if (hoveredObject != null)
                {
                    hoveredObject.OnHoverEnter();

                    if (hoveredObject.TryGetGrab(handType, transform.position, transform.up, transform.rotation, out var newPosition, out var newRotation))
                    {
                        grabIndicator.SetActive(true);
                        var lookToCameraDirection = _camera.transform.position - newPosition;
                        grabIndicator.UpdateTransform(newPosition, transform.position, Quaternion.LookRotation(lookToCameraDirection, Vector3.up));
                    } 
                    else
                    {
                        grabIndicator.SetActive(false);
                        hoveredObject = null;
                    }
                }
                else
                {
                    grabIndicator.SetActive(false);
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
                _physicsObject.Match(handController.transform.position, handController.transform.rotation);
            } 
            else
            {
                UpdateHeldObjectTransform();
            }
        }

        private void UpdateHeldObjectTransform()
        {
            grabbedObject.UpdateCachedValues(handType, handController.transform.position, handController.transform.up, handController.transform.rotation);
            _ = grabbedObject.UpdateTransformState(handType);
            grabbedObject.GetGrabHandPosition(handType, out var newPosition, out var newRotation);
            _rigidbody.position = newPosition;
            _rigidbody.rotation = newRotation;
        }
        #endregion

        #region inputTransmission
        private void OnMainReleased(object sender, EventArgs e)
        {
            if (IsGrabbingObject && grabbedObject is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnMainUp(handType);
            }
        }

        private void OnMainPressed(object sender, EventArgs e)
        {
            if (IsGrabbingObject && grabbedObject is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnMainDown(handType);
            }
        }

        private void OnTriggerChanged(object sender, float e)
        {
            if (IsGrabbingObject && grabbedObject is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnTriggerChange(handType, e);
            }
        }

        private void OnTriggerReleased(object sender, EventArgs e)
        {
            if (IsGrabbingObject && grabbedObject is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnTriggerUp(handType);
            }
        }

        private void OnTriggerPressed(object sender, EventArgs e)
        {
            if (IsGrabbingObject && grabbedObject is IInputSubscriber inputSubscriber)
            {
                inputSubscriber.OnTriggerDown(handType);
            }
        }
        #endregion

        #region Debug
        bool doDrawGizmos = false;
        private void OnDrawGizmos()
        {
            if (!doDrawGizmos)
            {
                return;
            }
            if (!ValidateDebug()) return;
            var gripColor = Color.Lerp(Color.red, Color.green, handController.GripValue);
            gripColor.a = 0.3f;
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
