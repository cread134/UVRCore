using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core.XRFramework.Physics
{
    public class PhysicsMover
    {
        private readonly XrObjectPhysicsConfig physicsConfiguration;
        private readonly Rigidbody _rigidbody;

        public PhysicsMover(XrObjectPhysicsConfig physicsConfiguration, Rigidbody rigidbody)
        {
            this.physicsConfiguration = physicsConfiguration;
            this._rigidbody = rigidbody;
            _rigidbody.maxAngularVelocity = physicsConfiguration.maxAngularVelocity;
            Reset();
        }

        public void Reset()
        {
        }

        public void MatchTransform(Transform targetTransform)
        {
            MatchTransform(targetTransform.position, targetTransform.rotation);
        }


        public void MatchTransform(Vector3 position, Quaternion rotation)
        {
            PhysicsMatchRotation(rotation);
            PhysicsMatchPosition(position);
        }

        public void PhysicsMatchPosition(Vector3 targetPosition)
        {
            Vector3 moveToHandVec = targetPosition - _rigidbody.transform.position;
            float neededSpeed = moveToHandVec.magnitude * (1.0f / Time.fixedDeltaTime);
            Vector3 neededSpeedVec = moveToHandVec.normalized * neededSpeed;
            Vector3 currentSpeedVec = _rigidbody.velocity;
            Vector3 newSpeedVec = neededSpeedVec - currentSpeedVec;
            _rigidbody.AddForce(newSpeedVec, ForceMode.VelocityChange);
        }

        public void PhysicsMatchRotation(Quaternion targetRotation)
        {
            Quaternion rotationChange = targetRotation * Quaternion.Inverse(_rigidbody.rotation);

            rotationChange.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f)
                angle -= 360f;

            if (Mathf.Approximately(angle, 0))
            {
                _rigidbody.angularVelocity = Vector3.zero;
                return;
            }

            angle *= Mathf.Deg2Rad;
            _rigidbody.angularVelocity = (axis * angle / Time.fixedDeltaTime);
            Debug.DrawLine(_rigidbody.position, _rigidbody.position + (_rigidbody.angularVelocity * 0.01f), Color.red);
        }
    }
}
