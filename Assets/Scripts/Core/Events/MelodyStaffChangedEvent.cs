namespace MutedMelody.Core.Events
{
    public struct MelodyStaffChangedEvent : IGameEvent
    {
        public float CurrentValue;
        public float MaxValue;
        public float Delta;
    }
}