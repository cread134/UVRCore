using System;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    public class TransformBinding : IBinding
    {
        Transform BindingTransform;
        PhysicsObject TargetTransform;
        IDisposable _disposable;

        public TransformBinding(PhysicsObject target, Transform bindingTarget)
        {
            TargetTransform = target;
            BindingTransform = bindingTarget;
            TargetTransform.transform.SetParent(BindingTransform, true);
            _disposable = target.OverridePhysics();
        }

        public void Break()
        {
            TargetTransform.transform.SetParent(null);
            _disposable.Dispose();
        }
    }
}
