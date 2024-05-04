using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsObject : MonoBehaviour
    {
        Rigidbody _rigidbody;
        public Rigidbody PhysicsRigidbody => _rigidbody ??= GetComponent<Rigidbody>();
        public float Mass = 1f;

        #region centreOfMass
        public Transform centreOfMassOverride;
        Vector3 CentreOfMass
        {
            get
            {
                if (centreOfMassOverride != null)
                {
                    return centreOfMassOverride.position;
                }
                return transform.position;
            }
        }

        public void ResetCentreOfMass()
        {
            var offset = CentreOfMass - PhysicsRigidbody.position;
            PhysicsRigidbody.centerOfMass = offset;
        }
        #endregion

        private void Awake()
        {
            PhysicsRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            PhysicsRigidbody.mass = Mass;
            ResetCentreOfMass();
            ResetVelocity();
        }

        public void ResetVelocity()
        {
            PhysicsRigidbody.velocity = Vector3.zero;
            PhysicsRigidbody.angularVelocity = Vector3.zero;
        }

        #region collision

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

        #endregion

        #region Debug
        private void OnDrawGizmos()
        {
            if (PhysicsRigidbody != null)
            {
                var handleLabelOffset = new Vector3(0, 0.005f, 0);
                var rigidbodyCentreOffMass = transform.TransformPoint(PhysicsRigidbody.centerOfMass);
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(rigidbodyCentreOffMass, 0.005f);

                Handles.Label(rigidbodyCentreOffMass + handleLabelOffset, "RB_COM");
                
                var referenceCentreOfMass = CentreOfMass;
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(referenceCentreOfMass, 0.005f);
                Handles.Label(referenceCentreOfMass + handleLabelOffset, "COM");
            }
        }
        #endregion
    }
}
