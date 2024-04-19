using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.XRFramework
{
    public enum HandType
    {
        Left,
        Right
    }

    public class HandController : MonoBehaviour
    {
        [SerializeField] private HandType _handType;
        public HandType HandType => _handType;

        private void Awake()
        {
            RegisterInputEvents();
        }

        #region Input
        [SerializeField] private InputActionReference _gripAction;
        [SerializeField] private InputActionReference _triggerAction;
        [SerializeField][Range(0, 1)] private float _gripThreshold = 0.5f;
        [SerializeField][Range(0, 1)] private float _triggerThreshold = 0.5f;

        float _gripValue;
        float _triggerValue;
        bool _gripPressed;
        bool _triggerPressed;

        public EventHandler<float> OnGripChangeEvent { get; set; }
        public EventHandler<float> OnTriggerChangeEvent { get; set; }
        public EventHandler OnGripPress { get; set; }
        public EventHandler OnGripRelease { get; set; }
        public EventHandler OnTriggerPress { get; set; }
        public EventHandler OnTriggerRelease { get; set; }
        public float GripValue => _gripValue;
        public float TriggerValue => _triggerValue;
        public bool GripPressed => _gripPressed;
        public bool TriggerPressed => _triggerPressed;

        void OnGripChange()
        {
            float newValue = _gripAction.action.ReadValue<float>();
            if (_gripValue < _gripThreshold && newValue >= _gripThreshold)
            {
                _gripPressed = true;
                OnGripPress?.Invoke(this, EventArgs.Empty);
            }
            else if (_gripValue >= _gripThreshold && newValue < _gripThreshold)
            {
                _gripPressed = false;
                OnGripRelease?.Invoke(this, EventArgs.Empty);
            }
            _gripValue = newValue;
            OnGripChangeEvent?.Invoke(this, _gripValue);
        }

        void OnTriggerChange()
        {
            float newValue = _triggerAction.action.ReadValue<float>();
            if (_triggerValue < _triggerThreshold && newValue >= _triggerThreshold)
            {
                _triggerPressed = true;
                OnTriggerPress?.Invoke(this, EventArgs.Empty);
            }
            else if (_triggerValue >= _triggerThreshold && newValue < _triggerThreshold)
            {
                _triggerPressed = false;
                OnTriggerRelease?.Invoke(this, EventArgs.Empty);
            }
            _triggerValue = newValue;
            OnTriggerChangeEvent?.Invoke(this, _triggerValue);
        }

        void RegisterInputEvents()
        {
            _gripAction.action.performed += ctx => OnGripChange();
            _gripAction.action.canceled += ctx => OnGripChange();
            _triggerAction.action.performed += ctx => OnTriggerChange();
            _triggerAction.action.canceled += ctx => OnTriggerChange();
        }
        #endregion
    }
}
