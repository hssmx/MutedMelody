namespace MutedMelody.Core.Events
{
    public struct BiomeColorChangedEvent : IGameEvent
    {
        public string BiomeID;
        public float NewSaturation; // 0.0 for Greyscale, 1.0 for Euphoric Color
    }
}