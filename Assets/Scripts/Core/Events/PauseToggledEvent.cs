namespace MutedMelody.Core.Events
{
    public struct PauseToggledEvent : IGameEvent
    {
        public bool IsPaused;
    }
}