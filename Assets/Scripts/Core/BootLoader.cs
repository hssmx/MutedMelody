using UnityEngine;
using UnityEngine.SceneManagement;

namespace MutedMelody.Core
{
    /// <summary>
    /// Core system responsible for ensuring the PersistentManagers scene is loaded.
    /// </summary>
    public class BootLoader : MonoBehaviour
    {
        private const string MANAGERS_SCENE_NAME = "PersistentManagers";

        protected virtual void Awake()
        {
            if (!IsManagerSceneLoaded())
            {
                Debug.Log($"[BootLoader] {MANAGERS_SCENE_NAME} not found. Loading additively...");
                SceneManager.LoadScene(MANAGERS_SCENE_NAME, LoadSceneMode.Additive);
            }
        }

        private bool IsManagerSceneLoaded()
        {
            // Iterate through all currently loaded scenes to see if our managers are already there
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).name == MANAGERS_SCENE_NAME)
                {
                    return true;
                }
            }
            return false;
        }
    }
}