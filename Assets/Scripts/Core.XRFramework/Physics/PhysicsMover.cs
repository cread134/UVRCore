using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Reset();
        }

        Quaternion lastRotationChange;
        Vector3 lastPositionChange;
        public void Reset()
        {
            lastRotationChange = Quaternion.identity;
            lastPositionChange = Vector3.zero;
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
            Quaternion deltaRotation = rotationChange * Quaternion.Inverse(lastRotationChange);
            lastRotationChange = rotationChange;

            deltaRotation.ToAngleAxis(out float deltaAngle, out Vector3 deltaAxis);
            deltaAngle *= physicsConfiguration.rotationalDelta;

            if (deltaAngle > 180f)
                deltaAngle -= 360f;

            rotationChange.ToAngleAxis(out float angle, out Vector3 axis);
            if (angle > 180f)
                angle -= 360f;

            if (Mathf.Approximately(angle, 0))
            {
                _rigidbody.angularVelocity = Vector3.zero;
                return;
            }

            angle *= Mathf.Deg2Rad;
            deltaAngle *= Mathf.Deg2Rad;

            var targetAngularVelocity = (axis * angle) / Time.deltaTime;

            float catchUp = 1.0f;
            targetAngularVelocity *= catchUp;
            var target = targetAngularVelocity - _rigidbody.angularVelocity;
            Vector3.SmoothDamp(_rigidbody.angularVelocity, targetAngularVelocity, ref target, (physicsConfiguration.torqueSmoothing * 0.1f));
            _rigidbody.AddTorque(target, ForceMode.VelocityChange);
        }
    }
}
