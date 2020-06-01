using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GodUnityPlugin
{
    public class Pooler<T> where T : Behaviour
    {
        private Dictionary<T, List<T>> poolPairs = new Dictionary<T, List<T>>();

        public Pooler()
        {
        }

        public Pooler(uint preloadCount, params T[] origins)
        {
            foreach (var origin in origins)
                InitializePool(origin, preloadCount);
        }

        public T Get(T origin)
        {
            if (!poolPairs.ContainsKey(origin))
                InitializePool(origin, 1);

            List<T> pool = poolPairs[origin];

            foreach (var t in pool)
            {
                if (t.gameObject.activeSelf)
                    continue;

                t.gameObject.SetActive(true);
                t.transform.parent = null;

                return t;
            }

            T newT = Object.Instantiate(origin);
            pool.Add(newT);

            return newT;
        }

        public void AddPool(T origin, uint preloadCount)
        {
            InitializePool(origin, preloadCount);
        }

        public void Pool(T origin)
        {
            if (!poolPairs.ContainsKey(origin))
                return;

            List<T> pool = poolPairs[origin];

            foreach (var go in pool)
            {
                if (go.gameObject.activeSelf)
                    go.gameObject.SetActive(false);
            }
        }

        public void PoolAll()
        {
            foreach (var pair in poolPairs)
                Pool(pair.Key);
        }

        private void InitializePool(T origin, uint preloadCount)
        {
            List<T> pool = new List<T>();

            for (int i = 0; i < preloadCount; i++)
            {
                T t = Object.Instantiate(origin);
                t.gameObject.SetActive(false);
                pool.Add(t);
            }

            poolPairs.Add(origin, pool);
        }
    }
}