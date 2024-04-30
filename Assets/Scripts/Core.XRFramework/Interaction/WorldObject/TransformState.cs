using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    public struct TransformState
    {
        public TransformState(Vector3 position, Vector3 upDirection, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
            UpDirection = upDirection;
        }
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 UpDirection;
    }
}
