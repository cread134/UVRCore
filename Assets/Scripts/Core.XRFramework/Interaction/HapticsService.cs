using Core.Service.DependencyManagement;
using Core.XRFramework.Context;
using Core.XRFramework.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace Core.XRFramework
{
    internal class HapticsService : SingletonClass<HapticsService>, IHapticsService
    {
        XrContext fContext;
        XrContext Context
        {
            get
            {
                return fContext ??= FindObjectOfType<XrContext>();
            }
        }
        public void SendHapticsImpulse(HandType handType, float amplitude, float duration)
        {
            var controller = Context?.GetController(handType);
            if (controller != null)
            {
                controller.SendHapticsImpulse(amplitude, duration);
            }
        }

        protected override void OnCreated()
        {
        }
    }
}
