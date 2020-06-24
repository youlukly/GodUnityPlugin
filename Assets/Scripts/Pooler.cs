using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GodUnityPlugin
{
    public class Pooler<T> where T : Behaviour
    {
        private Dictionary<T, List<T>> poolPairs = new Dictionary<T, List<T>>();
        private Dictionary<T, Transform> poolParentPairs = new Dictionary<T, Transform>();

        public Pooler()
        {
        }

        public Pooler(int preloadCount, params T[] origins)
        {
            foreach (var origin in origins)
                InitializePool(origin, preloadCount);
        }

        public Pooler(int preloadCount, List<T> origins)
        {
            foreach (var origin in origins)
                InitializePool(origin, preloadCount);
        }

        public Pooler(int preloadCount, string sceneName ,params T[] origins)
        {
            foreach (var origin in origins)
                InitializePool(origin, preloadCount, sceneName);
        }

        public Pooler(int preloadCount, string sceneName, List<T> origins)
        {
            foreach (var origin in origins)
                InitializePool(origin, preloadCount,sceneName);
        }

        public T Get(T origin)
        {
            if (!poolPairs.ContainsKey(origin))
                InitializePool(origin, 1);

            if (!poolParentPairs.ContainsKey(origin) || poolParentPairs[origin].Equals(null))
                CreatePoolParent(origin);

            Transform parent = poolParentPairs[origin];

            parent = poolParentPairs[origin];

            List<T> pool = poolPairs[origin];

            foreach (var t in pool)
            {
                if (t.gameObject.activeSelf)
                    continue;

                t.gameObject.SetActive(true);
                t.transform.parent = parent;

                return t;
            }

            T newT = Object.Instantiate(origin, parent);
            pool.Add(newT);

            return newT;
        }

        public void AddPool(T origin, int preloadCount)
        {
            InitializePool(origin, preloadCount);
        }

        public void AddPool(T origin, int preloadCount,string sceneName)
        {
            InitializePool(origin, preloadCount,sceneName);
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

        private void InitializePool(T origin, int preloadCount)
        {
            CreatePoolParent(origin);

            List<T> pool = new List<T>();

            for (int i = 0; i < preloadCount; i++)
            {
                T t = Object.Instantiate(origin, poolParentPairs[origin]);
                t.gameObject.SetActive(false);
                pool.Add(t);
            }

            poolPairs.Add(origin, pool);
        }

        private void InitializePool(T origin, int preloadCount,string sceneName)
        {
            CreatePoolParent(origin,sceneName);

            List<T> pool = new List<T>();

            for (int i = 0; i < preloadCount; i++)
            {
                T t = Object.Instantiate(origin, poolParentPairs[origin]);
                t.gameObject.SetActive(false);
                pool.Add(t);
            }

            poolPairs.Add(origin, pool);
        }

        private void CreatePoolParent(T origin)
        {
            poolParentPairs.Add(origin, new GameObject(origin.name + " " + "Pool").transform);
        }

        private void CreatePoolParent(T origin,string sceneName)
        {
            GameObject parent = new GameObject(origin.name + " " + "Pool");
            SetScene(parent,sceneName);
            poolParentPairs.Add(origin, parent.transform);
        }

        private void SetScene(GameObject go,string sceneName)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);

                if (scene.name != sceneName)
                    continue;

                SceneManager.MoveGameObjectToScene(go, scene);
            }
        }
    }
}