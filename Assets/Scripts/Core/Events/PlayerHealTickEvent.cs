namespace MutedMelody.Core.Events
{
    public struct PlayerHealTickEvent : IGameEvent
    {
        public float HealAmount;
        public float DeltaTime;
    }
}
