using Core.XRFramework.Interaction;
using Core.XRFramework.Interaction.WorldObject;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.XRFramework
{
    [SelectionBase]
    public class GrabPoint : MonoBehaviour
    {
        public GrabbableObject parent;
        public GrabPointGroup group;
        public HandType handType;

        public float maxGrabDistance = 0.05f;
        public float requiredMatchAngle = 35f;
        public bool CanGrabPoint(Vector3 referencePosition, Quaternion referenceRotation, out float priority)
        {
            var distance = Vector3.Distance(referencePosition, transform.position);
            var angle = Quaternion.Angle(referenceRotation, transform.rotation);
            priority = (0.1f * (distance + 1)) * (0.1f * (angle + 1));
            if (distance > (maxGrabDistance * 2f))
            {
                return false;
            }
            if (angle > requiredMatchAngle)
            {
                return false;
            }
            return true;
        }

        public TransformState GetGrabTransform(Vector3 referencePosition, Vector3 referenceUp, Quaternion referenceRotation)
        {
            return new(transform.position, transform.up, transform.rotation);
        }

        #region Debug

        private void OnDrawGizmos()
        {
            var semiTransparentColor = handType == HandType.Left ? new Color(1, 0, 0, 0.3f) : new Color(0, 0, 1, 0.3f);
            var solidColor = handType == HandType.Left ? new Color(1, 0, 0, 1) : new Color(0, 0, 1, 1);
            Gizmos.color = semiTransparentColor;
            Gizmos.DrawWireSphere(transform.position, maxGrabDistance);
            Gizmos.color = solidColor;
            Gizmos.DrawSphere(transform.position, 0.005f);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.02f);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.02f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.right * 0.01f);

            var label = $"{handType} {maxGrabDistance}";
            var labelOffset = new Vector3(0, 0.01f, 0);
            Handles.Label(transform.position + labelOffset, label);
            Gizmos.DrawIcon(transform.position, "emptygizmo.png", true);

            DrawAddGizmos();
        }

        protected virtual void DrawAddGizmos()
        {
        } 
        #endregion
    }
}
