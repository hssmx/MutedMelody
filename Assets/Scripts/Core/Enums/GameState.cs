namespace MutedMelody.Core
{
    /// <summary>
    /// Defines the overarching state of the application and gameplay loop.
    /// Used by the GameStateManager to control system behavior.
    /// </summary>
    public enum GameState
    {
        MainMenu,
        Loading,
        Playing,
        Paused,
        Cutscene,
        BossFight,
        Dead,
        Respawning
    }
}