using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Core.Networking
{

    public class GameNetwork : MonoBehaviour
    {
        public GameObject NetworkCanvas;

        private void Awake()
        {
            if (NetworkCanvas != null)
            {
                NetworkCanvas.SetActive(true);
            }
        }
        public void StartServer()
        {
            NetworkManager.Singleton.StartServer();
            NetworkCanvas.SetActive(false);
        }

        public void StartClient()
        {
            NetworkManager.Singleton.StartClient();
            NetworkCanvas.SetActive(false);
        }

        public void StartHost()
        {
            NetworkManager.Singleton.StartHost();
            NetworkCanvas.SetActive(false);
        }
    }
}
