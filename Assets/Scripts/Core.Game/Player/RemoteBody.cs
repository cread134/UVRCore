using UnityEngine;

namespace Core.Game
{
    public class RemoteBody : MonoBehaviour
    {
        public Transform target;

        public void Update()
        {
            if (target)
            {
                SyncToTarget();
            }
        }

        void SyncToTarget()
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}
