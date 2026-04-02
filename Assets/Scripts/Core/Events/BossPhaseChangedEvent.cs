namespace MutedMelody.Core.Events
{
    public struct BossPhaseChangedEvent : IGameEvent
    {
        public int NewPhase;
        public float HpThreshold;
    }
}