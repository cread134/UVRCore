using UnityEngine;

namespace Core.XRFramework.Physics
{
    [RequireComponent(typeof(PhysicsObject))]
    public class TestPhysicsMover : MonoBehaviour
    {
        public PhysicsObject physicsObject;
        public Transform targetTransform;

        private void Start()
        {
            physicsObject = GetComponent<PhysicsObject>();
        }

        private void FixedUpdate()
        {
            physicsObject.Match(targetTransform);
        }
    }
}
