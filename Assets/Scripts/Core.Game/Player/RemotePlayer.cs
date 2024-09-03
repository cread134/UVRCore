using Core.XRFramework.Context;
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
            GetComponent<XrContext>().enabled = true;
            Debug.Log("Remote player setup");
        }
    }
}
