using System.Collections.Generic;
using UnityEngine;

namespace MutedMelody.Core.Pooling
{
    public class PoolManager : Singleton<PoolManager>
    {
        // We use 'object' here because dictionaries cannot store generic types with different constraints
        private readonly Dictionary<string, object> _pools = new Dictionary<string, object>();

        public void CreatePool<T>(string poolName, T prefab, int initialSize, Transform parent = null) where T : Component
        {
            if (_pools.ContainsKey(poolName))
            {
                Debug.LogWarning($"[PoolManager] Pool with name '{poolName}' already exists. Skipping creation.");
                return;
            }

            ObjectPool<T> newPool = new ObjectPool<T>(prefab, initialSize, parent ?? transform);
            _pools.Add(poolName, newPool);
        }

        public T Get<T>(string poolName) where T : Component
        {
            if (_pools.TryGetValue(poolName, out object poolObj))
            {
                if (poolObj is ObjectPool<T> pool)
                {
                    return pool.Get();
                }
                Debug.LogError($"[PoolManager] Pool '{poolName}' is not of type {typeof(T)}.");
            }
            else
            {
                Debug.LogError($"[PoolManager] No pool found with name '{poolName}'. Did you forget to call CreatePool?");
            }
            return null;
        }

        public void Return<T>(string poolName, T obj) where T : Component
        {
            if (_pools.TryGetValue(poolName, out object poolObj))
            {
                if (poolObj is ObjectPool<T> pool)
                {
                    pool.Return(obj);
                    return;
                }
            }
            Debug.LogError($"[PoolManager] Failed to return object to pool '{poolName}'.");
        }

        public void ReturnAll<T>(string poolName) where T : Component
        {
            if (_pools.TryGetValue(poolName, out object poolObj))
            {
                if (poolObj is ObjectPool<T> pool)
                {
                    pool.ReturnAll();
                }
            }
        }
    }
}