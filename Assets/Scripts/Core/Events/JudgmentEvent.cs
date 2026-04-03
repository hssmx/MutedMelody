namespace MutedMelody.Core.Events
{
    public struct JudgmentEvent : IGameEvent
    {
        public JudgmentResult Result;
        public float OffsetMs;
    }
}
