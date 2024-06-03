using Core.Service.Physics;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    public class ObjectInput : MonoBehaviour, IGrabOverrider
    {
        [SerializeField] private Transform inputPoint;
        [SerializeField] private float inputRadius = 0.01f;
        [SerializeField] private float inputDistance = 0.01f;
        [SerializeField] private LayerMaskConfiguration inputMask;
        [SerializeField] private float verticalAngleAllowance = 25f;
        [SerializeField] private float horizontalAngleAllowance = 25f;
        [SerializeField] private bool inputOpen = true;

        private float startInputBuffer;

        IObjectInputSubscriber _subscriber;
        bool IsInputting
        {
            get
            {
                return _subscriber != null;
            }
            set
            {
                if (!value)
                {
                    _subscriber = null;
                }
            }
        }

        void Update()
        {
            if (!IsInputting && inputOpen)
            {
                CheckForInput();
            } 
            else
            {
                CheckToEndInput();
            }
        }

        RaycastHit[] inputHits = new RaycastHit[1];
        void CheckForInput()
        {
            var rayHits = UnityEngine.Physics.SphereCastNonAlloc(inputPoint.position, inputRadius, inputPoint.forward, inputHits, 0.01f, inputMask.LayerMask, QueryTriggerInteraction.Ignore);
            if (rayHits > 0 && inputHits[0].transform.gameObject.TryGetComponent(out IObjectInputSubscriber subscriber))
            {
                if (IsValidInput(subscriber))
                {
                    SetInput(subscriber);
                }
            }
        }

        bool IsValidInput(IObjectInputSubscriber subscriber)
        {
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
        }

        public (Vector3, Quaternion) GetOverrideTransform(GrabOverrideRefValues refValues)
        {
            return (refValues.BodyPosition, refValues.BodyRotation);
        }

        void CheckToEndInput()
        {
            if (_subscriber == null)
            {
                return;
            }
            var dot = Vector3.Dot(inputPoint.forward, _subscriber.InputReferencePoint.forward);
            var distance = Vector3.Distance(_subscriber.InputReferencePoint.position, inputPoint.position);
            if (distance > inputDistance + startInputBuffer && dot < 0.5f)
            {
                ClearInput();
            }
        }

        void ClearInput()
        {
            if (!IsInputting)
            {
                return;
            }
            _subscriber.OnInputEnd();
            _subscriber.AttachedGrab.ReleaseOverride();
            IsInputting = false;
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
            }
        }
        #endregion
    }
}
