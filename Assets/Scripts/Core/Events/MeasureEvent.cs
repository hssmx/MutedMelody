namespace MutedMelody.Core.Events
{
    public struct MeasureEvent : IGameEvent
    {
        public int MeasureNumber;
        public double DspTime;
    }
}