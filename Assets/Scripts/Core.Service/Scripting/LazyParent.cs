using UnityEngine;

namespace Core.Service.Scripting
{
    /// <summary>
    /// Load a component from the parent hierarchy of a transform
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazyParent<T> where T : MonoBehaviour
    {
        private T _component;
        private Transform root;

        public LazyParent(Transform root)
        {
            this.root = root;
        }

        public T Value
        {
            get
            {
                return _component ??= QueryParentRecursive(root);
            }
        }

        T QueryParentRecursive(Transform current)
        {
            if (current == null)
            {
                Debug.LogError($"Component {typeof(T).Name} not found in parent hierarchy");
                return null;
            }
            if (current.TryGetComponent(out T component))
            {
                return component;
            }
            return QueryParentRecursive(current.parent);
        }
    }
}
