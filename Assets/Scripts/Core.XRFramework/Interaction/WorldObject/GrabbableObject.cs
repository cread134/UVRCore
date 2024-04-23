using Core.XRFramework.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    [RequireComponent(typeof(PhysicsObject))]
    internal class GrabbableObject : MonoBehaviour, IGrabbableObject
    {
        public Transform Transform => transform;

        public void OnHoverEnter()
        {
        }

        public void OnHoverExit()
        {
        }

        public bool TryGetGrab(Transform referenceTransform, out Transform newTransform)
        {
            newTransform = transform;
            return false;
        }
    }
}
