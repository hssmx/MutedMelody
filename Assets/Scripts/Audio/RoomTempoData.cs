using UnityEngine;

namespace MutedMelody.Audio
{
    [CreateAssetMenu(fileName = "NewRoomTempo", menuName = "Muted Melody/Audio/Room Tempo Data")]
    public class RoomTempoData : ScriptableObject
    {
        public float bpm = 120f;
        public int beatsPerMeasure = 4;
        public int beatValue = 4;
        [Range(0f, 1f)] public float swingAmount = 0f;
    }
}
