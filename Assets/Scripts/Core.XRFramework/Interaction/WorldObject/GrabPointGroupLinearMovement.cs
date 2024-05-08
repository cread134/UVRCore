using Core.XRFramework.Interaction;
using Core.XRFramework.Interaction.WorldObject;
using UnityEngine;
using UnityEngine.Events;

namespace Core.XRFramework
{
    public class GrabPointGroupLinearMovement : GrabPointGroup, IMoveableGrabPoint
    {
        [Header("MovementSettings")]
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private float _smoothing = 7f;

        public UnityEvent OnHitStart = new ();
        public UnityEvent OnHitEnd = new();
        public UnityEvent<float> OnHitMove = new();

        private Vector3 offset;
        private Vector3 onGrabOffset;
        float pointDistance;
        private bool locked;

        public bool Locked { get => locked; set => locked = value; }

        private void Awake()
        {
            offset = _startPoint.position - transform.position;
            pointDistance = Vector3.Distance(_startPoint.position, _endPoint.position);
        }

        internal override void OnGrabbed(HandType handType, GrabPoint grabPoint, Vector3 referencePosition, Quaternion referenceRotation)
        {
            onGrabOffset = referencePosition - grabPoint.transform.position;
            base.OnGrabbed(handType, grabPoint, referencePosition, referenceRotation);
        }

        public override bool TryGetGrabPosition(HandType handType, Vector3 referencePosition, Quaternion referenceRotation, out GrabPoint grabPoint)
        {
            if (base.TryGetGrabPosition(handType, referencePosition, referenceRotation, out grabPoint))
            {
                UpdateGrabPointPosition(grabPoint, referencePosition);
                return true;
            }
            return false;
        }

        void UpdateGrabPointPosition(GrabPoint grabPoint, Vector3 referencePosition)
        {
            var referenceWithOffset = referencePosition - offset + onGrabOffset;
            var projectedVector = Vector3.Project(referenceWithOffset - _startPoint.position, _endPoint.position - _startPoint.position);
            var clampedVector = Vector3.ClampMagnitude(projectedVector, pointDistance);
            var targetPosition = _startPoint.position + clampedVector;
            grabPoint.transform.position = Vector3.Lerp(transform.position, targetPosition + offset, Time.deltaTime * _smoothing);
            if (Vector3.Distance(transform.position, transform.position + onGrabOffset) < onGrabOffset.magnitude)
            {
                onGrabOffset = transform.position - referencePosition;
            }
        }

        private void OnDrawGizmos()
        {
            if (_startPoint == null || _endPoint == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(_startPoint.position, _endPoint.position);
        }
    }
}
