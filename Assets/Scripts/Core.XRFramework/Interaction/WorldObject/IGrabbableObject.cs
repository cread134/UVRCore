using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    internal interface IGrabbableObject
    {
        Transform Transform { get; }

        bool CanGrab();
        void OnHoverEnter();
        void OnHoverExit();
        void OnGrab(Vector3 referencePosition, Quaternion referenceRotation);
        void OnRelease(Vector3 referencePosition, Quaternion referenceRotation);
        bool TryGetGrab(Vector3 referencePosition, Quaternion referenceRotation, out Vector3 newPosition, out Quaternion newRotation);
        void GetGrabPosition(Vector3 referencePosition, Quaternion referenceRotation, out Vector3 newPosition, out Quaternion newRotation);
        void UpdateTransform(Vector3 targetPosition, Quaternion targetRotation);
    }
}