using UnityEngine;

namespace MutedMelody.Stats
{
    [CreateAssetMenu(fileName = "DefaultMelodyStaffData", menuName = "Muted Melody/Stats/Melody Staff Data")]
    public class MelodyStaffData : ScriptableObject
    {
        [Header("Core")]
        public float maxMelody = 100f;
        public float startingMelody = 50f;
        
        [Header("Beat Rewards")]
        public float perfectBeatReward = 10f;
        public float goodBeatReward = 5f;
        
        [Header("Combat")]
        public float enemyKillReward = 15f;
        public float damageAmount = 20f;
        
        [Header("Abilities")]
        public float tuneDrain = 15f;
        public float tuneHeal = 20f;
        public float dashCost = 25f;
        
        [Header("Respawn")]
        [Range(0f, 1f)] public float respawnPercent = 0.5f;
    }
}
