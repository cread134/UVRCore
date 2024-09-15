using UnityEngine;

namespace Core.XRFramework.Physics
{

    public class PhysicsBinding : IBinding
    {
        Joint BindingJoint;

        public PhysicsBinding(PhysicsObject target, PhysicsObject bindingTarget)
        {
            BindingJoint = target.gameObject.AddComponent<FixedJoint>();
            BindingJoint.connectedBody = bindingTarget.PhysicsRigidbody;
        }

        public void Break()
        {
            Object.Destroy(BindingJoint);
        }
    }
}
