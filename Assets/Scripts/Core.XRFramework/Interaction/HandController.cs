using Core.Service.DependencyManagement;
using Core.Service.Logging;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

namespace Core.XRFramework.Interaction
{

    [SelectionBase]
    public class HandController : MonoBehaviour
    {
        [SerializeField] HandType _handType;
        public HandType HandType => _handType;

        LazyService<ILoggingService> loggingService = new LazyService<ILoggingService>();

        private void Awake()
        {
            RegisterInputEvents();
        }

        #region Input
        [SerializeField] private ActionBasedController _xrController;
        [SerializeField] private InputActionReference _gripAction;
        [SerializeField] private InputActionReference _triggerAction;
        [SerializeField] private InputActionReference _mainButtonAction;
        [SerializeField] private InputActionReference _secondaryButtonAction;
        [SerializeField][Range(0, 1)] private float _gripThreshold = 0.5f;
        [SerializeField][Range(0, 1)] private float _triggerThreshold = 0.5f;

        public EventHandler<float> OnGripChangeEvent { get; set; }
        public EventHandler<float> OnTriggerChangeEvent { get; set; }
        public EventHandler OnGripPress { get; set; }
        public EventHandler OnGripRelease { get; set; }
        public EventHandler OnTriggerPress { get; set; }
        public EventHandler OnTriggerRelease { get; set; }
        public EventHandler OnMainButtonDownEvent { get; set; }
        public EventHandler OnMainButtonUpEvent { get; set; }
        public EventHandler OnSecondaryButtonDownEvent { get; set; }
        public EventHandler OnSecondaryButtonUpEvent { get; set; }

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
        public bool GripPressed { get; set; } = false;
        public bool TriggerPressed { get; set; } = false;

        void OnGripChange()
        {
            float newValue = _gripAction.action.ReadValue<float>();
            OnGripChange(newValue);
        }

        void OnGripChange(float newValue)
        {
            if (newValue >= _gripThreshold)
            {
                if (!GripPressed)
                {
                    OnGripPress?.Invoke(this, EventArgs.Empty);
                }
                GripPressed = true;
            }
            else if ( newValue < _gripThreshold - 0.1)
            {
                if (GripPressed)
                {
                    OnGripRelease?.Invoke(this, EventArgs.Empty);
                }
                GripPressed = false;
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
            _mainButtonAction.action.performed += ctx => OnMainButtonDown();
            _mainButtonAction.action.canceled += ctx => OnMainButtonUp();
            _secondaryButtonAction.action.performed += ctx => OnSecondaryButtonDown();
            _secondaryButtonAction.action.canceled += ctx => OnSecondaryButtonUp();
        }

        public void OnMainButtonDown()
        {
            loggingService.Value?.Log($"{HandType} Main Button Down");
            OnMainButtonDownEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnMainButtonUp()
        {
            loggingService.Value?.Log($"{HandType} Main Button Up");
            OnSecondaryButtonUpEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnSecondaryButtonDown()
        {
            loggingService.Value?.Log($"{HandType} Secondary Button Down");
            OnSecondaryButtonDownEvent?.Invoke(this, EventArgs.Empty);
        }

        public void OnSecondaryButtonUp()
        {
            loggingService.Value?.Log($"{HandType} Secondary Button Up");
            OnSecondaryButtonUpEvent?.Invoke(this, EventArgs.Empty);
        }

        internal void SendHapticsImpulse(float amplitude, float duration)
        {
           _xrController.SendHapticImpulse(amplitude, duration);
        }
        #endregion
    }
}
