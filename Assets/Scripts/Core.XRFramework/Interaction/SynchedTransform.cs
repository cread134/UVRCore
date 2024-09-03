using Unity.Netcode;
using UnityEngine;

namespace Core.XRFramework.Interaction
{
    public class SynchedTransform : NetworkBehaviour
    {
        [SerializeField] float lerpSpeed = 18f;

        public void Sync(Transform reference)
        {
            if (!IsOwner)
            {
                return;
            }
            transform.position = Vector3.Lerp(transform.position, reference.position, Time.deltaTime * lerpSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, reference.rotation, Time.deltaTime * lerpSpeed);
        }
    }
}
