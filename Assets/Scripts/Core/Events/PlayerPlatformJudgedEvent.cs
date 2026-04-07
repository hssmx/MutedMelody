namespace MutedMelody.Core.Events
{
    public struct PlayerPlatformJudgedEvent : IGameEvent
    {
        public JudgmentResult Result;
        public float OffsetMs;
    }
}
