using Core.Service.Physics;
using Core.Service.Scripting;
using Core.XRFramework.Interaction.WorldObject;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    public class InputPoint : MonoBehaviour, IObjectInputSubscriber
    {
        [SerializeField] private Transform inputPoint;

        LazyParent<GrabbableObject> attachedGrab;

        public IGrabbableObject AttachedGrab => attachedGrab.Value;
        public Transform InputReferencePoint => inputPoint;

        [SerializeField] private float inputRadius = 0.01f;

        private void Awake()
        {
            attachedGrab = new LazyParent<GrabbableObject>(transform);
            SetupPhysics();
        }

        void SetupPhysics()
        {
            gameObject.layer = (int)LayerInfo.InputObject;
            var collider = gameObject.AddComponent<SphereCollider>();
            collider.radius = inputRadius;
        }
        public void OnInputStart()
        {
        }

        public void OnInputEnd()
        {
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
