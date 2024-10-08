﻿using Core.XRFramework.Physics;
using System;
using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    public interface IGrabbableObject
    {
        Transform Transform { get; }
        PhysicsObject PhysicsObject { get; }
        EventHandler ReleaseRequested { get; set; }
        bool IsTransformOverriden { get; }
        bool IsBeingGrabbed { get; }
        ulong OwnerId { get; }

        bool CanGrab(ulong clientId);
        void OnHoverEnter();
        void OnHoverExit();
        void OnGrab(HandType handType, Vector3 referencePosition, Vector3 referenceUp, Quaternion referenceRotation);
        void OnRelease(HandType handType, Vector3 referencePosition, Quaternion referenceRotation);
        bool TryGetGrab(HandType handType, Vector3 referencePosition, Vector3 referenceUp, Quaternion referenceRotation, out Vector3 newPosition, out Quaternion newRotation);
        void GetGrabHandPosition(HandType handType, out Vector3 newPosition, out Quaternion newRotation);
        void UpdateCachedValues(HandType handType, Vector3 targetPosition, Vector3 upDirection, Quaternion targetRotation);
        (Vector3 newPositionTarget, Quaternion newRotationTarget) UpdateTransformState(HandType handType);
        void SetOverride(IGrabOverrider grabOverride, bool disableCollision = false);
        void ReleaseOverride();
        void DoRelease();
        void SetGrabbedServerCmd(ulong id, bool value);
    }
}