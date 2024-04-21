using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Core.XRFramework.Interaction
{
    public enum HandType
    {
        Left,
        Right
    }

    public class HandController : MonoBehaviour
    {
        [SerializeField] HandType _handType;
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

        public EventHandler<float> OnGripChangeEvent { get; set; }
        public EventHandler<float> OnTriggerChangeEvent { get; set; }
        public EventHandler OnGripPress { get; set; }
        public EventHandler OnGripRelease { get; set; }
        public EventHandler OnTriggerPress { get; set; }
        public EventHandler OnTriggerRelease { get; set; }

        float _gripValue;
        public float GripValue
        {
            get {
                return _gripValue;
            }
            set {
                OnGripChange(value);
            }
        }

        float _triggerValue;
        public float TriggerValue 
        {
            get
            {
                return _triggerValue;
            }
            set
            {
                OnTriggerChange(value);
            }
        }
        public bool GripPressed { get; set; }
        public bool TriggerPressed { get; set; }

        void OnGripChange()
        {
            float newValue = _gripAction.action.ReadValue<float>();
            OnGripChange(newValue);
        }

        void OnGripChange(float newValue)
        {
            if (_gripValue < _gripThreshold && newValue >= _gripThreshold)
            {
                GripPressed = true;
                OnGripPress?.Invoke(this, EventArgs.Empty);
            }
            else if (GripValue >= _gripThreshold && newValue < _gripThreshold)
            {
                GripPressed = false;
                OnGripRelease?.Invoke(this, EventArgs.Empty);
            }
            _gripValue = newValue;
            OnGripChangeEvent?.Invoke(this, GripValue);
        }

        void OnTriggerChange()
        {
            float newValue = _triggerAction.action.ReadValue<float>();
            OnTriggerChange(newValue);
        }

        void OnTriggerChange(float newValue)
        {
            if (TriggerValue < _triggerThreshold && newValue >= _triggerThreshold)
            {
                TriggerPressed = true;
                OnTriggerPress?.Invoke(this, EventArgs.Empty);
            }
            else if (_triggerValue >= _triggerThreshold && newValue < _triggerThreshold)
            {
                TriggerPressed = false;
                OnTriggerRelease?.Invoke(this, EventArgs.Empty);
            }
            _triggerValue = newValue;
            OnTriggerChangeEvent?.Invoke(this, TriggerValue);
        }
        void RegisterInputEvents()
        {
            _gripAction.action.performed += ctx => OnGripChange();
            _gripAction.action.canceled += ctx => OnGripChange();
            _triggerAction.action.performed += ctx => OnTriggerChange();
            _triggerAction.action.canceled += ctx => OnTriggerChange();
        }

        public void OnMainButtonDown()
        {
            Debug.Log($"{HandType} Main Button Down");
        }
        public void OnSecondaryButtonDown()
        {
            Debug.Log($"{HandType} Secondary Button Down");
        }
        #endregion
    }
}
