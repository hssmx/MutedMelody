using UnityEngine;

namespace MutedMelody.Core.Events
{
    public struct CheckpointActivatedEvent : IGameEvent
    {
        public string CheckpointID;
        public Vector2 Position;
    }
}