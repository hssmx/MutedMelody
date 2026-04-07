using UnityEngine;

namespace MutedMelody.Core
{
    /// <summary>
    /// Generic singleton base for manager-style MonoBehaviours.
    /// For this project, managers live in PersistentManagers and are loaded once.
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static bool HasInstance
        {
            get
            {
                if (_instance != null)
                    return true;

                _instance = FindFirstObjectByType<T>();
                return _instance != null;
            }
        }

        public static bool TryGetInstance(out T instance)
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();
            }

            instance = _instance;
            return instance != null;
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        Debug.LogError($"[Singleton] Critical Error: No instance of {typeof(T)} found in the scene hierarchy.");
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} detected on GameObject '{gameObject.name}'. Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
