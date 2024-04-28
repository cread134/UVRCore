using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsObject : MonoBehaviour
    {
        Rigidbody _rigidbody;
        public Rigidbody PhysicsRigidbody => _rigidbody;

        private void Awake()
        {    
            _rigidbody = GetComponent<Rigidbody>();
        }

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
