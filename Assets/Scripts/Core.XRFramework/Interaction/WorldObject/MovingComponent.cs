using UnityEngine;

namespace Core.XRFramework.Interaction.WorldObject
{
    [System.Serializable]
    public class MovingComponent : MonoBehaviour
    {
        public Transform Target => transform;
        [HideInInspector] public Vector3 offset;

        public bool ApplyMovement { get; set; } = true;
    }
}
