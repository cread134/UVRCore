using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    internal interface IGrabbableObject
    {
        Transform Transform { get; }

        void OnHoverEnter();
        void OnHoverExit();
        bool TryGetGrab(Transform referenceTransform, out Transform newTransform);
    }
}