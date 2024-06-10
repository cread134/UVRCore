using Core.Service.Physics;
using Core.Service.Scripting;
using Core.XRFramework.Interaction.WorldObject;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    public class InputPoint : MonoBehaviour, IObjectInputSubscriber
    {
        [SerializeField] private float inputRadius = 0.01f;
        [SerializeField] private List<Collider> inputColliders;

        LazyParent<GrabbableObject> attachedGrab;
        bool _isConnected;

        public bool IsConnected => _isConnected;
        public IGrabbableObject AttachedGrab => attachedGrab.Value;
        public Transform InputReferencePoint => transform;

        private void Awake()
        {
            attachedGrab = new LazyParent<GrabbableObject>(transform);
            SetupPhysics();
        }

        void SetupPhysics()
        {
            gameObject.layer = (int)LayerInfo.InputObject;
            var collider = gameObject.AddComponent<SphereCollider>();

            collider.isTrigger = true;
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            collider.radius = inputRadius;
        }

        public void OnInputStart()
        {
            _isConnected = true;
            EnableInputColliders(false);
        }

        public void OnInputEnd()
        {
            _isConnected = false;
            EnableInputColliders(true);
        }

        void EnableInputColliders(bool enable) 
        { 
            inputColliders.ForEach(x => x.enabled = enable);
        }

        #region Debug
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, 0.005f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.01f);
            Gizmos.DrawLine(transform.position, transform.position + transform.right * 0.01f);

            Gizmos.DrawIcon(transform.position, "emptygizmo.png", true);
        }
        #endregion
    }
}
