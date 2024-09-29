using Core.Service.AudioManagement;
using Core.Service.DependencyManagement;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    [SelectionBase]
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsObject : NetworkBehaviour
    {
        Rigidbody _rigidbody;
        public Rigidbody PhysicsRigidbody => _rigidbody ??= GetComponent<Rigidbody>();
        public float Mass = 1f;

        public float VelocityMagnitude => PhysicsRigidbody.linearVelocity.magnitude;
        public Vector3 Velocity => PhysicsRigidbody.linearVelocity;

        public bool IsGrabbed { get; set; }

        public bool IsKinematic
        {
            get => PhysicsRigidbody.isKinematic;
            set => PhysicsRigidbody.isKinematic = value;
        }


        [SerializeField] 
        private XrObjectPhysicsConfig physicsConfiguration;
        public PhysicsMover PhysicsMover { get; set; }

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
            //PhysicsMover.Reset();
        }
        #endregion

        private void Awake()
        {
            PhysicsMover = new PhysicsMover(physicsConfiguration, PhysicsRigidbody);
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
            //PhysicsMover.Reset();
        }

        public IDisposable OverridePhysics()
        {
            return new PhysicsObjectOverride(this);
        }

        #region moving

        public void Match(Transform targetTransform)
        {
            PhysicsMover.MatchTransform(targetTransform);
        }

        public void Match(Vector3 position, Quaternion rotation)
        {
            PhysicsMover.MatchTransform(position, rotation);
        }

        public void SetPosition (Vector3 position)
        {
            ResetVelocity();
            PhysicsMover.SetPosition(position);
        }

        public void SetRotation(Quaternion rotation)
        {
            ResetVelocity();
            PhysicsMover.SetRotation(rotation);
        }
        
        #endregion

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

        Dictionary<GameObject, IBinding> _bindings = new Dictionary<GameObject, IBinding>();

        /// <summary>
        /// Makes joint to bind to object
        /// </summary>
        /// <param name="bindingTarget"></param>
        public void BindTo(PhysicsObject bindingTarget)
        {
            if (!_bindings.ContainsKey(bindingTarget.gameObject))
            {
                _bindings.Add(bindingTarget.gameObject, new PhysicsBinding(this, bindingTarget));
                Debug.Log($"Binding {bindingTarget} to {this}");
            } 
            else
            {
                Debug.LogWarning($"Already bound to {bindingTarget}");
            }
        }

        /// <summary>
        /// Binds to transform
        /// </summary>
        /// <param name="bindingParent"></param>
        public void BindTo(Transform bindingParent)
        {
            if (!_bindings.ContainsKey(bindingParent.gameObject))
            {
                _bindings.Add(bindingParent.gameObject, new TransformBinding(this, bindingParent));
                Debug.Log($"Binding (transforM) {bindingParent} to {this}");
            } 
            else
            {
                Debug.LogWarning($"Already bound to {bindingParent}");
            }
        }

        public void ReleaseBinding(PhysicsObject bindingTarget)
        {
            if (_bindings.ContainsKey(bindingTarget.gameObject))
            {
                _bindings[bindingTarget.gameObject].Break();
                _bindings.Remove(bindingTarget.gameObject);
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
