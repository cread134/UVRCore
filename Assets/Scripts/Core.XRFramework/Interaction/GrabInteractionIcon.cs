using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Interaction
{
    public class GrabInteractionIcon : MonoBehaviour
    {
        private LineRenderer _lineRender;
        private void Awake()
        {
            _lineRender = GetComponent<LineRenderer>();
        }

        public void SetActive(bool active)
        {
            if (_lineRender != null)
            {
                _lineRender.enabled = active;
            }
        }
        public void UpdateTransform(Vector3 newPos, Vector3 endPos, Quaternion newRot)
        {
            _lineRender.SetPosition(0, newPos);
            _lineRender.SetPosition(1, endPos);
        }
    }
}
