namespace MutedMelody.Core.Events
{
    public struct MelodyStaffChangedEvent : IGameEvent
    {
        public float PreviousMelody;
        public float CurrentMelody;
        public float NormalizedMelody;
        public bool IsCritical;
        public bool IsEmpty;
        public bool IsDead;
        public MelodyChangeReason Reason;
    }
}
