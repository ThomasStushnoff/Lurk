using System;
using System.Collections.Generic;
using Interfaces;
using JetBrains.Annotations;
using Objects;
using UnityEngine;

namespace Managers
{
    public class PrefabManager : Singleton<PrefabManager>
    {
        [SerializeField] private Prefabs list;

        private readonly Dictionary<PrefabType, Prefab> _prefabs = new();
        private readonly Dictionary<PrefabType, Queue<GameObject>> _pools = new ();

        /// <summary>
        /// Static shortcut method for creating a prefab.
        /// </summary>
        /// <param name="prefab">The type of prefab to create.</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="active">The active state of the prefab.</param>
        public static GameObject Create(PrefabType prefab, Transform parent = null, bool active = true) =>
            Instance.Instantiate(prefab, parent, active);

        /// <summary>
        /// Overload for creating a prefab and returning a component.
        /// </summary>
        /// <param name="prefab">The type of prefab to create.</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="active">The active state of the prefab.</param>
        /// <typeparam name="T">The type of component to return.</typeparam>
        /// <returns>The component of the prefab.</returns>
        public static T Create<T>(PrefabType prefab, Transform parent = null, bool active = true) where T : Component
        {
            var newObject = Instance.Instantiate(prefab, parent, active);
            var component = newObject.GetComponent<T>();
            if (component == null)
                Debug.LogError($"Prefab {prefab} does not have component {typeof(T)}");
            
            return component;
        }

        private void Awake()
        {
            DontDestroyOnLoad(this);

            foreach (var prefab in list.prefabs)
            {
                _prefabs.Add(prefab.type, prefab);

                // If the prefab should be pooled, create a pool for it.
                if (!prefab.shouldPool) continue;
                var root = GameObject.Find(prefab.root);

                _pools.Add(prefab.type, new Queue<GameObject>());
                for (var i = 0; i < prefab.initialPoolSize; i++)
                {
                    var newObject = Instantiate(prefab.prefab, root.transform);
                    newObject.SetActive(false);
                    _pools[prefab.type].Enqueue(newObject);
                }
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// Creates a new instance of the prefab.
        /// </summary>
        /// <param name="prefab">The prefab type</param>
        /// <param name="parent">The parent transform.</param>
        /// <param name="active">The active state.</param>
        /// <returns>The created object.</returns>
        public GameObject Instantiate(PrefabType prefab, Transform parent, bool active)
        {
            var prefabData = _prefabs[prefab];

            GameObject newObject;
            if (prefabData.shouldPool)
            {
                var pool = _pools[prefab];

                if (!pool.Peek().activeSelf)
                {
                    // Use the object from the pool.
                    newObject = pool.Dequeue();
                    newObject.SetActive(true);

                    // Reset the transform.
                    newObject.transform.Reset(true, true);
                    // Call reset.
                    var poolObject = newObject.GetComponent<IPoolObject>();
                    poolObject?.Reset();
                }
                else
                {
                    // Create a new object.
                    newObject = Instantiate(prefabData.prefab);
                }

                // Re-add the object to the pool.
                pool.Enqueue(newObject);
            }
            else
            {
                newObject = Instantiate(prefabData.prefab, parent);
            }
            
            newObject.SetActive(active);

            if (prefabData.root != null)
            {
                var root = GameObject.Find(prefabData.root);
                if (root)
                {
                    newObject.transform.SetParent(root.transform, false);
                }
            }

            return newObject;
        }
    }

    public enum PrefabType
    {
        
    }

    [Serializable]
    public struct Prefab
    {
        [TitleHeader("Prefab Info")]
        public PrefabType type;
        public GameObject prefab;

        [TitleHeader("Spawning Info")]
        [CanBeNull] public string root;

        [TitleHeader("Pooling Settings")]
        public bool shouldPool;
        public int initialPoolSize;
    }
}
