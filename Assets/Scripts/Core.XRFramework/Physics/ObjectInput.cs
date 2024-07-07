using Core.Service.DependencyManagement;
using Core.Service.Logging;
using Core.Service.Physics;
using Core.XRFramework.Interaction.WorldObject;
using UnityEngine;
using UnityEngine.Events;

namespace Core.XRFramework.Physics
{
    public class ObjectInput : MonoBehaviour, IGrabOverrider
    {
        [Header("Input Settings")]
        public GrabbableObject ParentGrab;

        [SerializeField] private Transform inputPoint;
        [SerializeField] private Transform endPoint;

        [SerializeField] private float inputRadius = 0.01f;
        [SerializeField] private float inputDistance = 0.01f;
        [SerializeField] private LayerMaskConfiguration inputMask;
        [SerializeField] private float verticalAngleAllowance = 25f;
        [SerializeField] private float horizontalAngleAllowance = 25f;
        [SerializeField] private bool inputOpen = true;

        [SerializeField] private GrabPoint[] blockingGrabPoints;

        [Header("Events")]
        public UnityEvent OnInputStart = new();
        public UnityEvent OnInputEnd = new();
        public UnityEvent OnComplete = new();

        [Header("Binding Settings")]
        [SerializeField] private bool bindOnComplete = true;
        [SerializeField] private bool releaseOnComplete = true;

        private float startInputBuffer;

        IObjectInputSubscriber _subscriber;

        LazyService<ILoggingService> loggingService = new();

        void Update()
        {
            if (!IsInputting)
            {
                if (ShouldCheckForInput())
                {
                    CheckForInput();
                }
            }
            else
            {
                CheckToEndInput();
            }
        }

        bool IsInputting
        {
            get => _subscriber != null;
            set
            {
                if (!value)
                {
                    _subscriber = null;
                }
            }
        }

        bool ShouldCheckForInput()
        {
            if (!inputOpen)
            {
                return false;
            }

            if (blockingGrabPoints != null)
            {
                foreach (var grabPoint in blockingGrabPoints)
                {
                    if (grabPoint.IsGrabbed)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        RaycastHit[] inputHits = new RaycastHit[1];
        void CheckForInput()
        {
            var rayHits = UnityEngine.Physics.SphereCastNonAlloc(inputPoint.position, inputRadius, inputPoint.forward, inputHits, 0.01f, inputMask.LayerMask, QueryTriggerInteraction.Collide);
            if (rayHits > 0)
            {
                if (inputHits[0].transform.gameObject.TryGetComponent(out IObjectInputSubscriber subscriber) && IsValidInput(subscriber))
                {
                    SetInput(subscriber);
                }
            }
        }

        bool IsValidInput(IObjectInputSubscriber subscriber)
        {
            if (subscriber == null)
            {
                return false;
            }

            if (subscriber.IsConnected)
            {
                return false;
            }

            var distance = Vector3.Distance(subscriber.InputReferencePoint.position, inputPoint.position);
            if (distance > inputDistance)
            {
                return false;
            }

            var sideAngle = Vector3.Angle(subscriber.InputReferencePoint.right, inputPoint.right);
            if (sideAngle > horizontalAngleAllowance)
            {
                return false;
            }

            var topAngle = Vector3.Angle(subscriber.InputReferencePoint.up, inputPoint.up);
            if (topAngle > verticalAngleAllowance)
            {
                return false;
            }

            return true;
        }

        void SetInput(IObjectInputSubscriber subscriber)
        {
            var dist = Vector3.Distance(subscriber.InputReferencePoint.position, inputPoint.position);
            startInputBuffer = dist + (dist * 0.1f); // 10% buffer for input start
            _subscriber = subscriber;
            _subscriber.OnInputStart();
            _subscriber.AttachedGrab.SetOverride(this);

            loggingService.Value.Log($"Input started for {_subscriber.AttachedGrab}");
            OnInputStart.Invoke();
        }

        public (Vector3, Quaternion) GetOverrideTransform(GrabOverrideRefValues refValues)
        {
            Vector3 targetPosition = CalculateOverridePosition(refValues);
            Quaternion targetRotation = CalculateOverrideRotation(refValues);
            return (targetPosition, targetRotation);
        }

        private Vector3 CalculateOverridePosition(GrabOverrideRefValues refValues)
        {
            var betweenDirection = (endPoint.position - inputPoint.position);

            var projectDirection = (refValues.TargetPosition - refValues.CurrentState.Position);
            var projectReference = _subscriber.InputReferencePoint.position + projectDirection;
            Debug.DrawLine(_subscriber.InputReferencePoint.position, projectReference, Color.blue);
            var projectedVector = Vector3.Project(projectDirection, betweenDirection);
            var projectedTarget = refValues.BodyPosition + projectedVector;
            Debug.DrawLine(refValues.BodyPosition, projectedTarget, Color.magenta);

            return projectedTarget;
        }

        private Quaternion CalculateOverrideRotation(GrabOverrideRefValues refValues)
        {
            var distanceBetweenInputAndStart = Vector3.Distance(_subscriber.InputReferencePoint.position, inputPoint.position);
            var lerpBetweenRotations = Quaternion.Lerp(inputPoint.rotation, endPoint.rotation, distanceBetweenInputAndStart / inputDistance);

            Quaternion C = inputPoint.rotation * Quaternion.Inverse(_subscriber.InputReferencePoint.rotation);
            Quaternion D = C * refValues.BodyRotation;
            return D;
        }
        void CheckToEndInput()
        {
            if (_subscriber == null)
            {
                return;
            }

            var endReferenceDot = Vector3.Dot(endPoint.position - inputPoint.position, _subscriber.InputReferencePoint.position - inputPoint.position);
            var distance = Vector3.Distance(_subscriber.InputReferencePoint.position, inputPoint.position);
            if (distance > inputDistance + startInputBuffer && endReferenceDot < 0f)
            {
                ClearInput();
            } 
            else if (distance > Vector3.Distance(endPoint.position, inputPoint.position))
            {
                OnCompleteInput();
            }
        }

        void OnCompleteInput()
        {
            if (releaseOnComplete)
            {
                _subscriber.AttachedGrab.DoRelease();
            }
            if (bindOnComplete)
            {
                ParentGrab.PhysicsObject.Bind(_subscriber.AttachedGrab.PhysicsObject);
            }
            OnComplete.Invoke();
        }

        void ClearInput()
        {
            if (!IsInputting)
            {
                return;
            }
            _subscriber.OnInputEnd();
            _subscriber.AttachedGrab.ReleaseOverride();
            loggingService.Value.Log($"Input ended for {_subscriber.AttachedGrab}");
            IsInputting = false;
            OnInputEnd.Invoke();
        }


        #region Debug
        private void OnDrawGizmos()
        {
            if (inputPoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(inputPoint.position, 0.005f);
                Gizmos.DrawLine(inputPoint.position, inputPoint.position + inputPoint.forward * 0.01f);
                Gizmos.DrawLine(inputPoint.position, inputPoint.position + inputPoint.right * 0.01f);

                if (IsInputting)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawLine(inputPoint.position, _subscriber.InputReferencePoint.position);
                }

                if (ShouldCheckForInput())
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(inputPoint.position, inputRadius);
                }

                if (endPoint != null)
                {
                    Gizmos.DrawLine(inputPoint.position, endPoint.position);
                }
            }
        }
        #endregion
    }
}
