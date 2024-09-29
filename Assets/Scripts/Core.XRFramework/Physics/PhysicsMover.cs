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
            Debug.Log("MatchTransform to " + position + "rot: " + rotation);
        }

        public void MatchTransform(Vector3 position, Quaternion rotation, PhysicsObject physicsObject)
        {
            PhysicsMatchRotationWithObject(rotation, physicsObject);
            PhysicsMatchPositionWithObject(position, physicsObject);
        }

        #region position
        public void PhysicsMatchPosition(Vector3 targetPosition)
        {
            var newSpeedVec = GetMatchVelocity(targetPosition);
            _rigidbody.AddForce(newSpeedVec, ForceMode.VelocityChange);
        }

        public void PhysicsMatchPositionWithObject(Vector3 targetPosition, PhysicsObject physicsObject)
        {
            var matchVelocity = GetMatchVelocity(targetPosition);
            var minimumVelocity = matchVelocity.magnitude * 0.4f;
            var maxVelocity = matchVelocity.magnitude;

            matchVelocity *= GetMassSlowdownMultipler(physicsObject);

            ClampVelocity(minimumVelocity, maxVelocity, ref matchVelocity);
            Vector3.SmoothDamp(_rigidbody.linearVelocity, matchVelocity, ref matchVelocity, physicsObject.Mass);
            _rigidbody.AddForce(matchVelocity, ForceMode.VelocityChange);
        }

        void ClampVelocity(float minMagnitude, float maxMagnitude, ref Vector3 velocity)
        {
            velocity = Vector3.ClampMagnitude(velocity, maxMagnitude);
            if (velocity.magnitude < minMagnitude)
            {
                velocity = velocity.normalized * minMagnitude;
            }
        }

        float GetMassSlowdownMultipler(PhysicsObject physicsObject)
        {
            var baseSlowdown = Mathf.Clamp01(1f / physicsObject.Mass);
            return baseSlowdown;
        }

        Vector3 GetMatchVelocity(Vector3 targetPosition)
        {
            Vector3 moveToHandVec = targetPosition - _rigidbody.transform.position;
            float neededSpeed = moveToHandVec.magnitude * (1.0f / Time.fixedDeltaTime);
            Vector3 neededSpeedVec = moveToHandVec.normalized * neededSpeed;
            Vector3 currentSpeedVec = _rigidbody.linearVelocity;
            Vector3 newVelocity = neededSpeedVec - currentSpeedVec;
            return newVelocity;
        }
        #endregion

        #region rotation

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
        }

        public void PhysicsMatchRotationWithObject(Quaternion targetRotation, PhysicsObject physicsObject)
        {
            PhysicsMatchRotation(targetRotation);
        }

        #endregion

        public void SetPosition(Vector3 position)
        {
            _rigidbody.position = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            _rigidbody.rotation = rotation;
        }
    }
}
