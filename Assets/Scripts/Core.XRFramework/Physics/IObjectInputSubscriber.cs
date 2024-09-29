using Core.XRFramework.Interaction.WorldObject;
using UnityEngine;

namespace Core.XRFramework.Physics
{
    public interface IObjectInputSubscriber
    {
        public InputKey InputKey { get; }
        public IGrabbableObject AttachedGrab { get; }
        public Transform InputReferencePoint { get; }
        public bool IsConnected { get; }

        void OnInputEnd();
        public void OnInputStart();
    }
}
