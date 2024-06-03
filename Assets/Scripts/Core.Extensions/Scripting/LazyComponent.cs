using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.DevTools.Scripting
{
    /// <summary>
    /// Load a component from a game object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyComponent<T> where T : Component
    {
        private T component;
        private GameObject gameObject;

        public LazyComponent(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public T Value
        {
            get
            {
                if (component == null)
                {
                    component = gameObject.GetComponent<T>();
                }
                return component;
            }
        }
    }
}
