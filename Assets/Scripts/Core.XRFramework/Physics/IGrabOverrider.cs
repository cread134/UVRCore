using UnityEngine;

namespace Core.XRFramework.Physics
{
    public interface IGrabOverrider 
    {
        public (Vector3, Quaternion) GetOverrideTransform(GrabOverrideRefValues refValues);
    }
}
