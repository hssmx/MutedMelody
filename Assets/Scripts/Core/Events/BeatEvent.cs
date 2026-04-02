namespace MutedMelody.Core.Events
{
    public struct BeatEvent : IGameEvent
    {
        public int BeatNumber;
        public int MeasureNumber;
        public double DspTime;
    }
}