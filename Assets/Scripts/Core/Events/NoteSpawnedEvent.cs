using UnityEngine;

namespace MutedMelody.Core.Events
{
    public struct NoteSpawnedEvent : IGameEvent
    {
        public Vector2 Position;
        public int ActiveCount;
        public double SpawnDspTime;
    }
}
