using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    public interface IGrabbableObject
    {
        Transform Transform { get; }

        bool CanGrab();
        void OnHoverEnter();
        void OnHoverExit();
        void OnGrab(HandType handType, Vector3 referencePosition, Quaternion referenceRotation);
        void OnRelease(HandType handType, Vector3 referencePosition, Quaternion referenceRotation);
        bool TryGetGrab(HandType handType, Vector3 referencePosition, Quaternion referenceRotation, out Vector3 newPosition, out Quaternion newRotation);
        void GetGrabHandPosition(HandType handType, Vector3 referencePosition, Quaternion referenceRotation, out Vector3 newPosition, out Quaternion newRotation);
        void UpdateTransform(HandType handType, Vector3 targetPosition, Quaternion targetRotation);
    }
}