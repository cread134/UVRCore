using UnityEngine;

namespace Core.Game
{
    public class RemotePlayer : MonoBehaviour
    {
        public GameObject localObject;
        public GameObject remoteObject;

        public void Setup()
        {
            localObject.SetActive(false);
            remoteObject.SetActive(true);
            Debug.Log("Remote player setup");
        }
    }
}
