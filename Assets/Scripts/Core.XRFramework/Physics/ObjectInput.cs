using Core.Service.Physics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    public class ObjectInput : MonoBehaviour, IGrabOverrider
    {
        [SerializeField] private Transform inputPoint;

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
            if (IsInputting)
            {
                CheckForInput();
            }
        }

        void CheckForInput()
        {

        }

        void SetInput(IObjectInputSubscriber subscriber)
        {
            _subscriber = subscriber;
            _subscriber.AttachedGrab.SetOverride(this);
        }

        void ClearInput()
        {
            if (!IsInputting)
            {
                return;
            }
            _subscriber.AttachedGrab.ReleaseOverried();
            IsInputting = false;
        }
        public (Vector3, Quaternion) GetOverrideTransform(GrabOverrideRefValues refValues)
        {
            return (Vector3.zero, Quaternion.identity);
        }

        #region Debug
        private void OnDrawGizmos()
        {
            if (inputPoint != null)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(inputPoint.position, 0.01f);
            }
        }
        #endregion
    }
}
