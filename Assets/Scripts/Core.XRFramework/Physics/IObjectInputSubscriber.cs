using Core.XRFramework.Interaction.WorldObject;

namespace Core.XRFramework.Physics
{
    public interface IObjectInputSubscriber
    {
        public IGrabbableObject AttachedGrab { get; }
    }
}
