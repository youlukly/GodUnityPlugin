using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace GodUnityPlugin
{
    public class GameObjectPooler
    {
        private Dictionary<GameObject, List<GameObject>> poolPairs = new Dictionary<GameObject, List<GameObject>>();
        private Dictionary<GameObject, Transform> poolParentPairs = new Dictionary<GameObject, Transform>();

        public GameObjectPooler()
        {
        }

        public GameObjectPooler(int preloadCount, params GameObject[] origins)
        {
            foreach (var origin in origins)
                Initialize(origin, preloadCount);
        }

        public GameObjectPooler(int preloadCount, List<GameObject> origins)
        {
            foreach (var origin in origins)
                Initialize(origin, preloadCount);
        }

        public GameObjectPooler(int preloadCount, string sceneName, params GameObject[] origins)
        {
            foreach (var origin in origins)
                Initialize(origin, preloadCount, sceneName);
        }

        public GameObjectPooler(int preloadCount, string sceneName, List<GameObject> origins)
        {
            foreach (var origin in origins)
                Initialize(origin, preloadCount, sceneName);
        }

        public GameObject Get(GameObject origin)
        {
            if (!poolPairs.ContainsKey(origin))
                Initialize(origin, 1);

            if (!poolParentPairs.ContainsKey(origin) || poolParentPairs[origin].Equals(null))
                CreatePoolParent(origin);

            Transform parent = poolParentPairs[origin];

            parent = poolParentPairs[origin];

            List<GameObject> pool = poolPairs[origin];

            foreach (var t in pool)
            {
                if (t.gameObject.activeSelf)
                    continue;

                t.gameObject.SetActive(true);
                t.transform.parent = parent;

                return t;
            }

            GameObject newT = Object.Instantiate(origin, parent);
            pool.Add(newT);

            return newT;
        }

        public void AddPool(GameObject origin, int preloadCount)
        {
            if (poolPairs.ContainsKey(origin))
                Add(origin, preloadCount);
            else
                Initialize(origin, preloadCount);
        }

        public void AddPool(GameObject origin, int preloadCount, string sceneName)
        {
            if (poolPairs.ContainsKey(origin))
                Add(origin, preloadCount, sceneName);
            else
                Initialize(origin, preloadCount, sceneName);
        }

        public void Pool(GameObject origin)
        {
            if (!poolPairs.ContainsKey(origin))
                return;

            List<GameObject> pool = poolPairs[origin];

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

        private void Add(GameObject origin, int preloadCount)
        {
            if (poolPairs[origin].Count >= preloadCount)
                return;

            int diff = poolPairs[origin].Count - preloadCount;

            for (int i = 0; i < diff; i++)
                poolPairs[origin].Add(Instantiate(origin));
        }

        private void Add(GameObject origin, int preloadCount, string sceneName)
        {
            if (poolPairs[origin].Count >= preloadCount)
                return;

            int diff = poolPairs[origin].Count - preloadCount;

            for (int i = 0; i < diff; i++)
                poolPairs[origin].Add(Instantiate(origin));

            SetScene(poolParentPairs[origin].gameObject, sceneName);
        }

        private void Initialize(GameObject origin, int preloadCount)
        {
            CreatePoolParent(origin);

            List<GameObject> pool = new List<GameObject>();

            for (int i = 0; i < preloadCount; i++)
                pool.Add(Instantiate(origin));

            poolPairs.Add(origin, pool);
        }

        private void Initialize(GameObject origin, int preloadCount, string sceneName)
        {
            CreatePoolParent(origin, sceneName);

            List<GameObject> pool = new List<GameObject>();

            for (int i = 0; i < preloadCount; i++)
                pool.Add(Instantiate(origin));

            poolPairs.Add(origin, pool);
        }

        private GameObject Instantiate(GameObject origin)
        {
            GameObject t = Object.Instantiate(origin, poolParentPairs[origin]);
            t.gameObject.SetActive(false);
            return t;
        }

        private void CreatePoolParent(GameObject origin)
        {
            poolParentPairs.Add(origin, new GameObject(origin.name + " " + "Pool").transform);
        }

        private void CreatePoolParent(GameObject origin, string sceneName)
        {
            GameObject parent = new GameObject(origin.name + " " + "Pool");
            SetScene(parent, sceneName);
            poolParentPairs.Add(origin, parent.transform);
        }

        private void SetScene(GameObject go, string sceneName)
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