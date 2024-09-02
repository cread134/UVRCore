using Unity.Netcode;
using UnityEngine;

namespace Core.Game
{
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(LocalPlayer))]
    [RequireComponent(typeof(RemotePlayer))]
    public class GamePlayer : NetworkBehaviour
    {
        private void Awake()
        {
            if (IsHost || IsClient)
            {
                GetComponent<LocalPlayer>().Setup();
            }
            else
            {
                GetComponent<RemotePlayer>().Setup();
            }
        }
    }
}
