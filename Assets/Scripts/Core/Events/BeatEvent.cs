namespace MutedMelody.Core.Events
{
    public struct BeatEvent : IGameEvent
    {
        public int BeatNumber;
        public double DspTime;
    }
}
