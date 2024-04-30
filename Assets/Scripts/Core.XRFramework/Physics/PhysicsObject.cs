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
        public float Mass = 1f;

        private void Awake()
        {    
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            _rigidbody.mass = Mass;
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

        #region Debug
        private void OnDrawGizmos()
        {
            if (_rigidbody != null)
            {
                var centreOfMass = transform.TransformPoint(_rigidbody.centerOfMass);
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(centreOfMass, 0.01f);
            }
        }
        #endregion
    }
}
