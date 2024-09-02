using UnityEngine;

namespace Core.Game
{
    public class LocalPlayer : MonoBehaviour
    {
        public GameObject foreignObject;
        public GameObject localObject;

        public void Setup()
        {
            foreignObject.SetActive(false);
            localObject.SetActive(true);
            Debug.Log("Local player setup");
        }
    }
}
