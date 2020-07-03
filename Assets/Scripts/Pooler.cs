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
                Initialize(origin, preloadCount);
        }

        public Pooler(int preloadCount, List<T> origins)
        {
            foreach (var origin in origins)
                Initialize(origin, preloadCount);
        }

        public Pooler(int preloadCount, string sceneName ,params T[] origins)
        {
            foreach (var origin in origins)
                Initialize(origin, preloadCount, sceneName);
        }

        public Pooler(int preloadCount, string sceneName, List<T> origins)
        {
            foreach (var origin in origins)
                Initialize(origin, preloadCount,sceneName);
        }

        public T Get(T origin)
        {
            if (!poolPairs.ContainsKey(origin))
                Initialize(origin, 1);

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
            if (poolPairs.ContainsKey(origin))
                Add(origin, preloadCount);
            else
                Initialize(origin, preloadCount);
        }

        public void AddPool(T origin, int preloadCount,string sceneName)
        {
            if (poolPairs.ContainsKey(origin))
                Add(origin, preloadCount,sceneName);
            else
                Initialize(origin, preloadCount,sceneName);
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

        private void Add(T origin, int preloadCount)
        {
            if (origin == null)
                return;

            if (poolPairs[origin].Count >= preloadCount)
                return;

            int diff = poolPairs[origin].Count - preloadCount;

            for (int i = 0; i < diff; i++)
                poolPairs[origin].Add(Instantiate(origin));
        }

        private void Add(T origin, int preloadCount,string sceneName)
        {
            if (origin == null)
                return;

            if (poolPairs[origin].Count >= preloadCount)
                return;

            int diff = poolPairs[origin].Count - preloadCount;

            for (int i = 0; i < diff; i++)
                poolPairs[origin].Add(Instantiate(origin));

            SetScene(poolParentPairs[origin].gameObject, sceneName);
        }

        private void Initialize(T origin, int preloadCount)
        {
            if (origin == null)
                return;

            CreatePoolParent(origin);

            List<T> pool = new List<T>();

            for (int i = 0; i < preloadCount; i++)
                pool.Add(Instantiate(origin));

            poolPairs.Add(origin, pool);
        }

        private void Initialize(T origin, int preloadCount,string sceneName)
        {
            CreatePoolParent(origin,sceneName);

            List<T> pool = new List<T>();

            for (int i = 0; i < preloadCount; i++)
                pool.Add(Instantiate(origin));

            poolPairs.Add(origin, pool);
        }

        private T Instantiate(T origin)
        {
            T t = Object.Instantiate(origin, poolParentPairs[origin]);
            t.gameObject.SetActive(false);
            return t;
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