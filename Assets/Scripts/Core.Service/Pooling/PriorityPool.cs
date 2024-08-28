using System.Collections.Generic;
using UnityEngine;

namespace Core.Service.Pooling
{
    internal class PriorityPool
    {
        public int Priority { get; private set; } = 5;

        const int initialPoolSize = 10;
        const int poolGrowth = 5;
        private readonly GameObject effectObject;
        private readonly Transform parent;
        int currentPoolSize;
        
        Queue<GameObject> pool = new Queue<GameObject>();

        public PriorityPool(GameObject effectObject, Transform parent)
        {
            currentPoolSize = initialPoolSize;
            CreateInstance();
            this.effectObject = effectObject;
            this.parent = parent;
        }

        void GrowPriority()
        {
            Priority++;
            if (Priority > currentPoolSize)
            {
                GrowPool();
            }
        }

        void GrowPool()
        {
            currentPoolSize += poolGrowth;
            for (int i = 0; i < poolGrowth; i++)
            {
                CreateInstance();
            }
        }

        void CreateInstance()
        {
            // Create instance
            var instance = GameObject.Instantiate(effectObject, parent);
            instance.SetActive(false);
            pool.Enqueue(instance);
        }

        public GameObject GetInstance()
        {
            if (pool.Count == 0)
            {
                GrowPriority();
            }
            var instance = pool.Dequeue();
            pool.Enqueue(instance);
            return instance;
        }
    }
}
