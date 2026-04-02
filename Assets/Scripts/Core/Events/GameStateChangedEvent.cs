namespace MutedMelody.Core.Events
{
    public struct GameStateChangedEvent : IGameEvent
    {
        public GameState PreviousState;
        public GameState NewState;
    }
}