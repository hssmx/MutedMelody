using UnityEngine;

namespace MutedMelody.Core.Events
{
    public struct PlayerDeathEvent : IGameEvent
    {
        public Vector2 DeathPosition;
    }
}