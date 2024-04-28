using Core.XRFramework.Interaction;
using Core.XRFramework.Interaction.WorldObject;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.XRFramework
{
    public class GrabPoint : MonoBehaviour
    {
        public GrabbableObject parent;
        public HandType handType;
        public float maxGrabDistance = 0.05f;
        public float requiredMatchAngle = 20f;
        public bool CanGrabPoint(Vector3 referencePosition, Quaternion referenceRotation, out float priority)
        {
            var distance = Vector3.Distance(referencePosition, transform.position);
            var angle = Quaternion.Angle(referenceRotation, transform.rotation);
            priority = (0.1f * (distance + 1)) * (0.1f * (angle + 1));
            if (distance > maxGrabDistance)
            {
                return false;
            }
            if (angle > requiredMatchAngle)
            {
                return false;
            }
            return true;
        }

        public (Vector3 newPosition, Quaternion newRotation) GetGrabTransform(Vector3 referencePosition, Quaternion referenceRotation)
        {
            return (transform.position, transform.rotation);
        }

        #region Debug

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, maxGrabDistance);
            var label = $"{handType} {maxGrabDistance}";
            Handles.Label(transform.position, label);
            Gizmos.DrawIcon(transform.position, "GrabIcon.png", true);
        }
        #endregion
    }
}
