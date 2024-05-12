using Core.XRFramework.Interaction;
using Core.XRFramework.Interaction.WorldObject;
using UnityEngine;
using UnityEngine.Events;

namespace Core.XRFramework.Interaction.WorldObject
{
    public class GrabPointGroupLinearMovement : GrabPointGroup, IMoveableGrabPoint
    {
        [Header("MovementSettings")]
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private Transform _movingPoint;
        [SerializeField] private float _smoothing = 15f;
        [SerializeField] private MovingComponent[] movingComponents = new MovingComponent[0];

        public UnityEvent OnHitStart = new ();
        public UnityEvent OnHitEnd = new();
        public UnityEvent<float> OnHitMove = new();

        private Vector3 offset;
        private Vector3 onGrabOffset;
        float pointDistance;
        private bool locked;

        public bool Locked { get => locked; set => locked = value; }

        protected override void OnAwake()
        {
            pointDistance = Vector3.Distance(_startPoint.position, _endPoint.position);
            _movingPoint.position = _startPoint.position;
            InitMovingComponents();
        }

        public override bool AllowTwoHandedMovement => false;

        void InitMovingComponents()
        {
            foreach (var component in movingComponents)
            {
                component.offset = component.Target.position - _movingPoint.position;
            }
        }

        internal override void OnGrabbed(HandType handType, GrabPoint grabPoint, Vector3 referencePosition, Quaternion referenceRotation)
        {
            offset = _startPoint.position - grabPoint.transform.position;
            onGrabOffset = referencePosition - grabPoint.transform.position;
            base.OnGrabbed(handType, grabPoint, referencePosition, referenceRotation);
        }

        public override TransformState GetGrabTransform(Vector3 referencePosition, Vector3 referenceUp, Quaternion referenceRotation, GrabPoint grabPoint)
        {
            if (IsGrabbed)
            {
                UpdateGrabPointPosition(grabPoint, referencePosition);
            }
            return base.GetGrabTransform(referencePosition, referenceUp, referenceRotation, grabPoint);
        }

        void UpdateGrabPointPosition(GrabPoint grabPoint, Vector3 referencePosition)
        {
            var referenceWithOffset = referencePosition + offset + onGrabOffset;
            var projectedVector = Vector3.Project(referenceWithOffset - _startPoint.position, _endPoint.position - _startPoint.position);
            if (Vector3.Dot(projectedVector.normalized, (_endPoint.position - _startPoint.position).normalized) < 0)
            {
                projectedVector = Vector3.zero;
            }
            var clampedVector = Vector3.ClampMagnitude(projectedVector, pointDistance);
            var targetPosition = _startPoint.position + clampedVector;

            Debug.DrawLine(_startPoint.position, _endPoint.position, Color.yellow);
            Debug.DrawLine(_startPoint.position, referencePosition, Color.blue);
            Debug.DrawLine(_startPoint.position, targetPosition, Color.red);

            _movingPoint.position = Vector3.Lerp(_movingPoint.position, targetPosition, Time.deltaTime * _smoothing);
            foreach (var component in movingComponents)
            {
                if (component.ApplyMovement)
                    component.Target.position = _movingPoint.position + component.offset;
            }
            grabPoint.transform.position = _movingPoint.position - offset;
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
