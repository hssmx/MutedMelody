namespace MutedMelody.Core
{
    /// <summary>
    /// Drop this component into any gameplay scene (e.g., DevTestScene, Biomes) 
    /// to guarantee managers load when pressing Play directly from that scene.
    /// </summary>
    public class SceneBootstrapper : BootLoader
    {
        // Inherits the Awake() logic from BootLoader. 
        // No additional code is needed!
    }
}