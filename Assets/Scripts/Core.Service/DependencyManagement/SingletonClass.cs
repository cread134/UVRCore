using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Service.DependencyManagement
{
    public abstract class SingletonClass<T> : MonoBehaviour where T : SingletonClass<T>
    {
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = CreateInstance();
                }
                return _instance;
            }
        }
        private static T _instance;

        private static T CreateInstance()
        {
            if (UnityEngine.Application.isPlaying == false)
            {
                return null;
            }
            var instance = FindObjectOfType<T>();
            if (instance == null)
            {
                var go = new GameObject(typeof(T).Name);
                instance = go.AddComponent<T>();
                instance.OnInstanceCreating();
            }
            return instance;
        }

        virtual protected void OnInstanceCreating()
        {
            DontDestroyOnLoad(gameObject);
            OnCreated();
        }

        protected abstract void OnCreated();
    }
}
