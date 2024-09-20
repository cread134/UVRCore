using Core.Service.Physics;
using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    public interface IObjectSocket
    {
        void AttachObject(IGrabbableObject obj);
        bool IsHoldingObject();
        IGrabbableObject DetachObject();
        bool CanHoldObject(IGrabbableObject obj);
    }

    public class ObjectSocket : MonoBehaviour, IObjectSocket
    {
        public IGrabbableObject heldObject;
        public Vector3 SocketDimension = new Vector3(0.1f, 0.1f, 0.1f);

        public bool CanHoldObject(IGrabbableObject obj)
        {
            return !IsHoldingObject();
        }

        public void AttachObject(IGrabbableObject obj)
        {
            heldObject = obj;
            Debug.Log($"{obj} attached to socket {this.name}");
        }

        public bool IsHoldingObject()
        {
            return heldObject != null;
        }

        public IGrabbableObject DetachObject()
        {
            var obj = heldObject;
            heldObject = null;
            return obj;
        }

        #region Debug
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, SocketDimension);
        }
        #endregion
    }
}
