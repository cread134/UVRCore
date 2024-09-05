using Core.Service.AudioManagement;
using Core.Service.DependencyManagement;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsObject : NetworkBehaviour
    {
        Rigidbody _rigidbody;
        public Rigidbody PhysicsRigidbody => _rigidbody ??= GetComponent<Rigidbody>();
        public float Mass = 1f;

        public float VelocityMagnitude => PhysicsRigidbody.linearVelocity.magnitude;
        public Vector3 Velocity => PhysicsRigidbody.linearVelocity;

        public bool IsGrabbed { get; set; }

        [Header("Collision")]
        public GameSound defaultCollisionSound;

        LazyService<IAudioService> audioService = new LazyService<IAudioService>();

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
            if (PhysicsRigidbody.isKinematic)
            {
                return;
            }
            PhysicsRigidbody.linearVelocity = Vector3.zero;
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
            audioService.Value.PlaySound(defaultCollisionSound, transform.position);
        }

        private bool collisionActive = true;
        public bool CollisionActive
        {
            get => collisionActive; set
            {
                collisionActive = value;
                PhysicsRigidbody.detectCollisions = value;
            }
        }
        #endregion

        #region Binding

        Dictionary<PhysicsObject, PhysicsBinding> _bindings = new Dictionary<PhysicsObject, PhysicsBinding>();

        public void Bind(PhysicsObject bindingTarget)
        {
            if (!_bindings.ContainsKey(bindingTarget))
            {
                _bindings.Add(bindingTarget, new PhysicsBinding(this, bindingTarget));
                Debug.Log($"Binding {bindingTarget} to {this}");
            }
        }

        public void ReleaseBinding(PhysicsObject bindingTarget)
        {
            if (_bindings.ContainsKey(bindingTarget))
            {
                _bindings[bindingTarget].Break();
                _bindings.Remove(bindingTarget);
                Debug.Log($"Releasing {bindingTarget} from {this}");
            }
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
