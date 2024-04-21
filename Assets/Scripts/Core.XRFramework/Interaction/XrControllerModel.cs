using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Interaction
{
    public class XrControllerModel : MonoBehaviour
    {
        [SerializeField] private HandController controller;
        [SerializeField] private Transform gripModel;
        [SerializeField] private Transform triggerModel;
        [SerializeField] private Transform gripStart;
        [SerializeField] private Transform gripEnd;
        [SerializeField] private Transform triggerStart;
        [SerializeField] private Transform triggerEnd;

        private void Awake()
        {
            controller.OnGripChangeEvent += OnGripChange;
            controller.OnTriggerChangeEvent += OnTriggerChange;
        }

        private void OnGripChange(object sender, float e)
        {
            gripModel.localPosition = Vector3.Lerp(gripStart.localPosition, gripEnd.localPosition, e);
        }

        private void OnTriggerChange(object sender, float e)
        {
            triggerModel.localPosition = Vector3.Lerp(triggerStart.localPosition, triggerEnd.localPosition, e);
        }
    }
}
