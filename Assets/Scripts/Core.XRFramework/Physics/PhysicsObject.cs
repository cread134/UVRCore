using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsObject : MonoBehaviour
    {
        const float MIN_COLLISION_IMPULSE = 0.1f;
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.impulse.magnitude > MIN_COLLISION_IMPULSE)
            {
                OnCollide(collision);
            }
        }

        public virtual void OnCollide(Collision collision)
        {
        }
    }
}
