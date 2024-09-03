using Core.Service.ObjectManagement;
using Core.XRFramework.Context;
using Unity.Netcode;
using UnityEngine;

namespace Core.Game
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(LocalPlayer))]
    [RequireComponent(typeof(RemotePlayer))]
    [RequireComponent(typeof(PlayerResources))]
    [RequireComponent(typeof(XrContext))]
    public class GamePlayer : NetworkBehaviour
    {
        private void Start()
        {
            if (IsOwner)
            {
                GetComponent<LocalPlayer>().Setup();
            }
            else
            {
                GetComponent<RemotePlayer>().Setup();
            }

            SpawnBehaviours();
        }

        void SpawnBehaviours()
        {
            foreach (var behaviour in GetComponentsInChildren<ISpawnable>())
            {
                behaviour.Spawned();
            }
        }
    }
}
