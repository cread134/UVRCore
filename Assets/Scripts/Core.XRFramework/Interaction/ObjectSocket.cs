using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    public interface IObjectSocket
    {
    }

    public class ObjectSocket : MonoBehaviour, IObjectSocket
    {
        public IGrabbableObject heldObject;
        public Vector3 SocketDimension = new Vector3(0.1f, 0.1f, 0.1f);

        public void AttachObject(IGrabbableObject obj)
        {
            heldObject = obj;
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
