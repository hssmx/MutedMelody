using UnityEngine;

namespace MutedMelody.Platforms
{
    [CreateAssetMenu(fileName = "DefaultNotePlatformData", menuName = "Muted Melody/Note Platform Data")]
    public class NotePlatformData : ScriptableObject
    {
        public int maxActivePlatforms = 3;
        public Vector2 platformSize = new Vector2(2f, 0.3f);
        public float spawnOffsetY = -0.5f;
    }
}
