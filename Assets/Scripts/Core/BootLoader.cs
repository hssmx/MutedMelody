using UnityEngine;
using UnityEngine.SceneManagement;

namespace MutedMelody.Core
{
    /// <summary>
    /// Ensures PersistentManagers is loaded before gameplay scripts start relying on manager singletons.
    /// RuntimeInitializeOnLoadMethod(BeforeSceneLoad) runs before any Awake in the first scene.
    /// </summary>
    public class BootLoader : MonoBehaviour
    {
        public const string ManagersSceneName = "PersistentManagers";
        private static bool _requestedLoad;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void EnsureManagersSceneLoadedBeforeSceneAwake()
        {
            LoadManagersSceneIfNeeded();
        }

        protected virtual void Awake()
        {
            // Fallback for editor/dev situations where this object exists in a scene opened directly.
            LoadManagersSceneIfNeeded();
        }

        private static void LoadManagersSceneIfNeeded()
        {
            if (_requestedLoad || IsManagerSceneLoaded())
                return;

            _requestedLoad = true;
            SceneManager.LoadScene(ManagersSceneName, LoadSceneMode.Additive);
            Debug.Log($"[BootLoader] Requested additive load for '{ManagersSceneName}'.");
        }

        private static bool IsManagerSceneLoaded()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == ManagersSceneName)
                    return true;
            }

            return false;
        }
    }
}
