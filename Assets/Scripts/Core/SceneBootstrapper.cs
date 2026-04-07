namespace MutedMelody.Core
{
    /// <summary>
    /// Optional editor fallback. The real bootstrap now happens through BootLoader's
    /// RuntimeInitializeOnLoadMethod(BeforeSceneLoad), so this component can stay empty.
    /// </summary>
    public class SceneBootstrapper : BootLoader
    {
    }
}
