using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    public class PhysicsMover
    {
        private readonly XrObjectPhysicsConfig physicsConfiguration;
        private readonly Rigidbody _rigidbody;

        Vector3 positionError;
        Vector3 lastPositionError;

        public PhysicsMover(XrObjectPhysicsConfig physicsConfiguration, Rigidbody rigidbody)
        {
            this.physicsConfiguration = physicsConfiguration;
            this._rigidbody = rigidbody;
        }

        public void MatchTransform(Transform targetTransform)
        {
            PhysicsMatchPosition(targetTransform.position);
            PhysicsMatchRotation(targetTransform.rotation);
        }

        public void PhysicsMatchPosition(Vector3 targetPosition)
        {
            positionError = targetPosition - _rigidbody.position;

            Vector3 positionProportion = positionError * physicsConfiguration.positionIntegrationCompenstation;

            Vector3 derivativeGain = (positionError - lastPositionError) / Time.fixedDeltaTime;
            Vector3 positionDerivative = derivativeGain * physicsConfiguration.positionSmoothing;

            lastPositionError = positionError;

            Vector3 force = positionProportion + positionDerivative;
            _rigidbody.AddForce(force, ForceMode.Acceleration);
        }

        private Quaternion rotationError;
        private Quaternion lastRotation;
        private Quaternion lastRotationError;
        private float angleError;
        private Vector3 errorAxis;
        public void PhysicsMatchRotation(Quaternion targetRotation)
        {
            rotationError = targetRotation * Quaternion.Inverse(_rigidbody.rotation);

            rotationError.ToAngleAxis(out angleError, out errorAxis);
            errorAxis.Normalize();
            if (angleError > 180f)
            {
                angleError -= 360f;
            }

            Quaternion rotationImpulse = lastRotationError * Quaternion.Inverse(lastRotationError);
            rotationImpulse.ToAngleAxis(out float angleImpluse, out Vector3 impulseAxis);

            if (angleImpluse > 180f)
            {
                angleImpluse -= 360f;
            }

            Quaternion targetRotationImpulse = targetRotation * Quaternion.Inverse(lastRotation);
            targetRotationImpulse.ToAngleAxis(out float targetImpulseAngle, out Vector3 targetImpulseVector);

            if (targetImpulseAngle > 180f)
            {
                targetImpulseAngle -= 360f;
            }

            _rigidbody.angularVelocity *= physicsConfiguration.anglularSlowdown;

            Vector3 angularVelocity = (errorAxis * angleError) * Time.deltaTime * physicsConfiguration.rotationErrorCompenstation;
            Vector3 angularImpulse = ((impulseAxis * angleImpluse) / Time.fixedDeltaTime) * physicsConfiguration.rotationImpulseCompensation;
            Vector3 targetAngularImpulse = ((targetImpulseAngle * targetImpulseVector) * Time.fixedDeltaTime) * physicsConfiguration.rotationIntergral;

            angularVelocity += (_rigidbody.angularVelocity - angularVelocity) * physicsConfiguration.rotationProportionalGain * Time.deltaTime;
            angularVelocity += angularImpulse;
            angularVelocity += targetAngularImpulse;
            _rigidbody.AddTorque(angularVelocity, ForceMode.VelocityChange);

            lastRotation = targetRotation;
            lastRotationError = rotationError;
        }
    }
}
