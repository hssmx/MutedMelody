using UnityEngine;

namespace MutedMelody.Core
{
    /// <summary>
    /// A generic base class for MonoBehaviours that require a single instance.
    /// Designed for additive scene architectures (does NOT use DontDestroyOnLoad).
    /// </summary>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    
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
            // Duplicate Check
            if (_instance == null)
            {
                _instance = this as T;
                
                // CRITICAL ARCHITECTURE NOTE:
                // We intentionally omit DontDestroyOnLoad(gameObject) here.
                // Managers inheriting from this will live in a Core/Master scene 
                // that is additively loaded and never unloaded during gameplay.
            }
            else if (_instance != this)
            {
                Debug.LogWarning($"[Singleton] Duplicate instance of {typeof(T)} detected on GameObject '{gameObject.name}'. Destroying duplicate.");
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            // Clean up the reference if this specific instance is destroyed
            // (e.g., when the game is completely closed or the Core scene is force-reloaded)
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}