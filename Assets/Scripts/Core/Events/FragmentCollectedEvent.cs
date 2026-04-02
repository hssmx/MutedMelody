namespace MutedMelody.Core.Events
{
    public struct FragmentCollectedEvent : IGameEvent
    {
        public string FragmentID;
        public string BiomeID;
    }
}