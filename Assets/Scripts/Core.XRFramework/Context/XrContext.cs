using Core.XRFramework.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.XRFramework.Context
{
    public class XrContext : MonoBehaviour
    {
        [SerializeField] PhysicalHandInteractor leftHand;
        [SerializeField] PhysicalHandInteractor rightHand;
        [SerializeField] HandController rightHandController;
        [SerializeField] HandController leftHandController;


        public PhysicalHandInteractor GetHand(HandType side)
        {
            return side == HandType.Left ? leftHand : rightHand;
        }

        public HandController GetController(HandType side)
        {
            return side == HandType.Left ? leftHandController : rightHandController;
        }

        public Camera GetCamera()
        {
            return Camera.main;
        }
    }
}
