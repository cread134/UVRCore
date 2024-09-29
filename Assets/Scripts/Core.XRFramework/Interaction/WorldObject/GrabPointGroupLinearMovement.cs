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
        [SerializeField] private float springConstant = 0f;
        [SerializeField] private float damping = 1f;

        public UnityEvent OnHitStart = new ();
        public UnityEvent OnHitEnd = new();
        public UnityEvent<float> OnHitMove = new();

        private Vector3 offset;
        private Vector3 onGrabOffset;
        private Vector3 velocity;
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

        private void Update()
        {
            if (!IsGrabbed && !locked)
            {
                if (springConstant > 0.1f && Vector3.Distance(_startPoint.position, _movingPoint.position) < 0.1f)
                {
                    //get amount to moving to accord to the spring force
                    Vector3 displacement = _startPoint.position - _movingPoint.position;
                    Vector3 springForce = springConstant * displacement;
                    Vector3 dampingForce = -damping * velocity;
                    Vector3 totalForce = springForce + dampingForce;
                    Vector3 acceleration = totalForce;
                    velocity += acceleration * Time.deltaTime;

                    // Update position
                    _movingPoint.position += velocity * Time.deltaTime;

                    CheckForEvents(velocity);
                    foreach (var component in movingComponents)
                    {
                        if (component.ApplyMovement)
                            component.Target.position = _movingPoint.position + component.offset;
                    }
                }
            } else
            {
                velocity = Vector3.zero;
            }
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

            velocity = (_movingPoint.position - targetPosition) * Time.deltaTime;
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
            CheckForEvents(velocity);
        }

        float eventCooldown = 0.1f;
        float lastEvent = 0;
        bool atStart = false;
        bool atEnd = false;
        void CheckForEvents(Vector3 velocity)
        {
            var dot = Vector3.Dot(velocity.normalized, (_endPoint.position - _startPoint.position).normalized);
                var distance = Vector3.Distance(_movingPoint.position, _startPoint.position);
            if (distance < 0.03f && dot > 0f)
            {
                if (!atStart)
                {
                    OnHitStart.Invoke();
                    Debug.Log("OnHitStart");
                }
                atStart = true;
            }
            else if (distance > pointDistance - 0.03f && dot < 0f)
            {
                if (!atEnd)
                {
                    OnHitEnd.Invoke();
                    Debug.Log("OnHitEnd");
                }
                atEnd = true;
            }
            else
            {
                if (lastEvent + eventCooldown < Time.time)
                {
                    atStart = false;
                    atEnd = false;
                    OnHitMove.Invoke(distance / pointDistance);
                    lastEvent = Time.time;
                }
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
