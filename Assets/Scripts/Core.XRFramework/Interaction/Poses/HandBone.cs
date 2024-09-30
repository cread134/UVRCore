using UnityEngine;

namespace Core.XRFramework.Interaction.Poses
{
    public class HandBone
    {
        public string Key;
        public Transform Transform;
        public HandBoneType BoneType;
        public void SetTransform(Vector3 position, Quaternion rotation)
        {
            Transform.position = position;
            Transform.rotation = rotation;
        }
    }

}
