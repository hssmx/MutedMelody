using System.Collections.Generic;
using UnityEngine;

namespace MutedMelody.Core.Pooling
{
    public class ObjectPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Queue<T> _availableObjects = new Queue<T>();
        private readonly List<T> _activeObjects = new List<T>();

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;
            WarmUp(initialSize);
        }

        private void WarmUp(int capacity)
        {
            for (int i = 0; i < capacity; i++)
            {
                T newObj = CreateNewObject();
                _availableObjects.Enqueue(newObj);
            }
        }

        private T CreateNewObject()
        {
            T newObj = Object.Instantiate(_prefab, _parent);
            newObj.gameObject.SetActive(false);
            return newObj;
        }

        public T Get()
        {
            T obj;
            if (_availableObjects.Count > 0)
            {
                obj = _availableObjects.Dequeue();
            }
            else
            {
                Debug.LogWarning($"[ObjectPool] Pool for '{_prefab.name}' exhausted. Growing pool on demand.");
                obj = CreateNewObject();
            }

            _activeObjects.Add(obj);
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(T obj)
        {
            if (obj == null) return;

            obj.gameObject.SetActive(false);
            if (_activeObjects.Remove(obj))
            {
                _availableObjects.Enqueue(obj);
            }
        }

        public void ReturnAll()
        {
            // Copy to array to safely remove items from the list while iterating
            T[] activeCopy = _activeObjects.ToArray();
            foreach (T obj in activeCopy)
            {
                Return(obj);
            }
        }
    }
}